#============ Notes on usage: ================================
#
# Les fichiers image associés doivent être mis au dossier nommé "Images" sous le dossier local pointé par le paramètre "SourcePath"
# Après exécution, les images vont être tranferts dans le répertoire SharePoint "SiteCollectionImages/AideEnLigne".
# Example command line:
# .\PopulateHelpLists.ps1 -WebURL "http://verd226" -SourcePath "Z:\Sources\Proteca.Sharepoint\Scripts\"
#
#============================================================

param(
    $WebURL = "http://verd174", 
    $SourcePath = "C:\BUILD\PROTECA.NET\Sources\Proteca.Sharepoint\Scripts\"
)

Clear-Host


$web= Get-SPWeb $WebURL

$src_data_doc_path = Join-Path $SourcePath "aide.xml"
$src_data_doc = [xml](gc $src_data_doc_path)

WRITE-HOST "Importing data into list [Glossary]:"
$list = $web.Lists["Glossary"]
foreach($topic in $src_data_doc.aide.glossaire.mot)
{
    $newitem = $list.Items.Add() 
    $newitem["Title"] = $topic.libelle.Trim()
    $newitem["TopicKey"] = $topic.libelle.Trim()
	$newitem["TopicContent"] = $topic.definition.Trim()
	$newitem.Update()
    WRITE-HOST "Topic [" $topic.libelle.Trim() "] added to list ..."
}

WRITE-HOST "Importing data into list [OnlineHelp]:"
$list = $web.Lists["OnlineHelp"]
foreach($topic in $src_data_doc.aide.aideEnLigne.topic)
{
    $newitem = $list.Items.Add() 
    $newitem["Title"] = $topic.libelle.Trim()
    $newitem["TopicKey"] = $topic.libelle.Trim()
	$newitem["TopicContent"] = $topic.definition.Trim()
	$newitem.Update()
    WRITE-HOST "Topic [" $topic.libelle.Trim() "] added to list ..."
}

WRITE-HOST "Importing data into list [DiagnosticHelp]:"
$list = $web.Lists["DiagnosticHelp"]
foreach($group in $src_data_doc.aide.diagnostic.groupe)
{
    $folderUrl = Join-Path $list.RootFolder.ServerRelativeUrl $group.titre.Trim()
    $folder = $web.GetFolder($folderUrl)
    if (-not $folder.Exists)
    {
        $newitem = $list.Items.Add($list.RootFolder.ServerRelativeUrl, [Microsoft.SharePoint.SPFileSystemObjectType]::Folder, $group.titre.Trim())
        $newitem[[Microsoft.SharePoint.SPBuiltInFieldId]::ContentTypeId] = $list.ContentTypes["Groupe de sujet"].Id
	    $newitem["GroupDescription"] = $group.description.Trim()
	    $newitem.SystemUpdate()
        $folder = $newitem.Folder
        WRITE-HOST "Folder [" $newitem.Url "] created ..."
    }
   
    foreach($topic in $group.topic)
    {
        $newitem = $list.Items.Add($folder.ServerRelativeUrl, [Microsoft.SharePoint.SPFileSystemObjectType]::File, $topic.titre.Trim()) 

        $newitem[[Microsoft.SharePoint.SPBuiltInFieldId]::ContentTypeId] = $list.ContentTypes["Élement"].Id
        $newitem["TopicKey"] = $topic.titre.Trim()
	    $newitem["TopicContent"] = $topic.description.Trim()

	    $newitem.SystemUpdate()
        WRITE-HOST "---- Topic [" $topic.titre.Trim() "] added to folder: [" $folder.ServerRelativeUrl "]..."
    }   
}	

WRITE-HOST "Importing image fiels into [SiteCollectionImages/AideEnLigne]:"
$lib = $web.GetList("/SiteCollectionImages")
$folder_name = "AideEnLigne" 
$dest_path = Join-Path $lib.RootFolder.ServerRelativeUrl $folder_name 
write-host $dest_path
$dest_folder = $web.GetFolder($dest_path)
if (-not $dest_folder.Exists)
{
    $newitem = $lib.Items.Add($lib.RootFolder.ServerRelativeUrl, [Microsoft.SharePoint.SPFileSystemObjectType]::Folder, $folder_name)
    $newitem.SystemUpdate()
    Write-host  $newitem
    $dest_folder = $newitem.Folder
    WRITE-HOST "Folder [" $dest_folder.ServerRelativeUrl "] created ..."
} 
$image_path = Join-Path $SourcePath "Images"
$image_files = Get-ChildItem  $image_path
foreach ($image_file in $image_files)
{
    $stream = (Get-Item $image_file.FullName).OpenRead()
	$file = $dest_folder.Files.Add($image_file.Name, $stream, $TRUE)
    WRITE-HOST "---- Uploaded file [" $image_file.Name "] ..."
}

$web.Dispose()

