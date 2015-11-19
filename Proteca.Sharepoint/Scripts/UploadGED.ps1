# Notes on usage:
# 
# Example command line:
# .\UploadGED.ps1 -WebURL "http://VERD226" -SourcePath "Y:\"
#
# After execution, there will be a summary of each file uploaded, with complete path information, as well as the total number of files uploaded.

param(
    $WebURL, 
    $LibName = "GED",
    $SourcePath = "Z:\"
)

Clear-Host

function UploadFromLocalDir($WebURL, $LibName, $SourcePath)
{
	$web= Get-SPWeb $WebURL
    $sp_folder = $Web.GetFolder($LibName)
	$source_files = Get-ChildItem $SourcePath #You can filter files by: -filter "*.pdf"
    $manifest_doc_path = Join-Path $SourcePath  "manifest.xml"
    $manifest_doc = [xml](gc $manifest_doc_path -encoding UTF8)
    $c = 0
    foreach($document in $manifest_doc.Documents.Document)
	{
        $cle_pp = 0
        $cle_portion = 0
        $cle_equipment = 0
        $cle_ensemble_electrique = 0
        
        switch($document.typeEquipment)
        {
            "1" { $cle_pp = $document.cleOuvrage }
            "2" { $cle_portion = $document.cleOuvrage }
            "3" { $cle_equipment = $document.cleOuvrage }
            "4" { $cle_ensemble_electrique = $document.cleOuvrage }
        }
        
        $source_file_path = Join-Path $SourcePath $document.sourceFileName
		$stream = (Get-Item $source_file_path).OpenRead()
		$meta_data = @{
            "ClePP" = $cle_pp; 
            "ClePortion" = $cle_portion; 
            "CleEquipement" = $cle_equipment; 
            "CleEnsembleElectrique" = $cle_ensemble_electrique; 
            "Title" = $document.fileName;
            "Libellé" = $document.fileName;
            "NumEnregistrement" = $document.version;
            "Archive" = $document.isArchived
        }

   
        $dest_file_path = $LibName + "/" + $document.typeOuvrage + "/" + $document.typeDossier + "/" + $document.designation + "/" + $document.fileName
		$spfile = $sp_folder.Files.Add($dest_file_path, $stream, $meta_data, $TRUE)
        $spfile.Item["Created"] = Get-Date $document.date
        $spfile.Item["Modified"] = Get-Date $document.date
        $spfile.Item.Update()
		$stream.Dispose()
        # Remove previous verions
        $ver_count = $spfile.Versions.Count
        while ($spfile.Versions.Count -gt 0)
        {
            $version = $spfile.Versions[0]
            $version.Delete()
        }
        
        WRITE-HOST "Uploaded file:" $dest_file_path
        $c = $c + 1
	}
	$web.Dispose()
    WRITE-HOST "----------------- Summary -------------------"
    WRITE-HOST "Successfully uploaded" $c  "files."
}


UploadFromLocalDir $WebURL $LibName $SourcePath