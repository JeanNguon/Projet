<%@ Page language="C#" MasterPageFile="~masterurl/default.master" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server"> 
    <SharePoint:CSSRegistration name= "<%$SPUrl:~SiteCollection/Style Library/Proteca/help.css%>" After="corev4.css" runat="server"/>
    <SharePoint:ScriptLink language="javascript" name="SP.js"  LoadAfterUI="true" OnDemand="false" Localizable="false" runat="server"/>
    <SharePoint:ScriptLink language="javascript" name="Proteca/Scripts/jquery.js" LoadAfterUI="false" OnDemand="false" Localizable="false" runat="server"/>
    <SharePoint:ScriptLink language="javascript" name="Proteca/Scripts/UrlTool.js" LoadAfterUI="false" OnDemand="false" Localizable="false" runat="server"/>
    <SharePoint:ScriptLink language="javascript" name="Proteca/Scripts/HelpBase.js" LoadAfterUI="false" OnDemand="false" Localizable="false" runat="server"/>
    <SharePoint:ScriptLink language="javascript" name="Proteca/Scripts/OnlineHelp.js" LoadAfterUI="false" OnDemand="false" Localizable="false" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2 id="title"></h2>
    <div id="HelpContent">Veuillez patienter. Le contenu de l'aide est en cours de chargement...</div>
</asp:Content>