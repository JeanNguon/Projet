<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Proteca</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost 
    {
	    height:30px;
	    text-align:center;
	    position:relative;
	    z-index:0;
    }
    #mainContent 
    {
	    z-index:1;
    }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
              appSource = sender.getHost().Source;
            }
            
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
              return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " +  appSource + "\n" ;

            errMsg += "Code: "+ iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {           
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " +  args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

        
        function ToggleMenu(expand) {            
            host = document.getElementById("silverlightControlHost");
            
            if (expand) {
                host.style.zIndex = 2;
            }
            else {
                host.style.zIndex = 0;
            }
        }

        function resizeSilverlight() {
            if (document.getElementById('SilverlightWebPart')) {
                document.getElementById('SilverlightWebPart').height = "500px";
                for (var i = 0; i < document.getElementById('SilverlightWebPart').childNodes.length; i++) {
                    if (document.getElementById('SilverlightWebPart').childNodes[i].attributes != null && document.getElementById('SilverlightWebPart').childNodes[i].name.toLowerCase() == 'background') {
                        document.getElementById('SilverlightWebPart').childNodes[i].value = 'transparent';
                    }
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height:100%">
    <div id="headerContent">
        <h1>Lorem ipsum</h1>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque dignissim cursus risus vel interdum. Aenean eu augue erat, vel viverra urna. Quisque fermentum est sed turpis consequat semper. Morbi suscipit orci vitae mauris ultricies interdum faucibus eros varius. Integer eget convallis orci. Morbi volutpat arcu eu mauris eleifend accumsan. Morbi porta placerat odio, eget posuere purus scelerisque vitae. Etiam urna leo, porttitor eu imperdiet ut, malesuada vitae justo. Donec tincidunt mauris sit amet dui venenatis facilisis. In est elit, gravida id posuere vitae, ultrices sit amet nisl. Integer neque nisi, lobortis a iaculis quis, sodales et dui. In sagittis tempor tristique. Quisque ut lacus ac massa malesuada commodo at ac ligula. Sed quis aliquet neque. Nam pretium orci et purus semper aliquet.</p>
    </div>
    <div id="silverlightControlHost" onresize="resizeSilverlight();">
        <object id="SilverlightWebPart" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="300px">
		  <param name="source" value="ClientBin/Proteca.Silverlight.xap?date=<%=DateTime.Now.Ticks.ToString() %>"/>
		  <param name="onError" value="onSilverlightError" />
          <%--<param name="initParams" value="Jounce.LogLevel=Error,DebugLogin=REGION" />--%>
          <param name="initParams" value="Jounce.LogLevel=Information,DebugLogin=n.cossard" />
		  <param name="background" value="white" />
          <param name="windowless" value="true" />
		  <param name="minRuntimeVersion" value="5.0.61118.0" />
		  <param name="autoUpgrade" value="true" />
          <param name="windowless" value="true" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
	    </object>
        <iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>
    </div>
    <div id="mainContent">
        <h1>Lorem ipsum</h1>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque dignissim cursus risus vel interdum. Aenean eu augue erat, vel viverra urna. Quisque fermentum est sed turpis consequat semper. Morbi suscipit orci vitae mauris ultricies interdum faucibus eros varius. Integer eget convallis orci. Morbi volutpat arcu eu mauris eleifend accumsan. Morbi porta placerat odio, eget posuere purus scelerisque vitae. Etiam urna leo, porttitor eu imperdiet ut, malesuada vitae justo. Donec tincidunt mauris sit amet dui venenatis facilisis. In est elit, gravida id posuere vitae, ultrices sit amet nisl. Integer neque nisi, lobortis a iaculis quis, sodales et dui. In sagittis tempor tristique. Quisque ut lacus ac massa malesuada commodo at ac ligula. Sed quis aliquet neque. Nam pretium orci et purus semper aliquet.</p>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque dignissim cursus risus vel interdum. Aenean eu augue erat, vel viverra urna. Quisque fermentum est sed turpis consequat semper. Morbi suscipit orci vitae mauris ultricies interdum faucibus eros varius. Integer eget convallis orci. Morbi volutpat arcu eu mauris eleifend accumsan. Morbi porta placerat odio, eget posuere purus scelerisque vitae. Etiam urna leo, porttitor eu imperdiet ut, malesuada vitae justo. Donec tincidunt mauris sit amet dui venenatis facilisis. In est elit, gravida id posuere vitae, ultrices sit amet nisl. Integer neque nisi, lobortis a iaculis quis, sodales et dui. In sagittis tempor tristique. Quisque ut lacus ac massa malesuada commodo at ac ligula. Sed quis aliquet neque. Nam pretium orci et purus semper aliquet.</p>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque dignissim cursus risus vel interdum. Aenean eu augue erat, vel viverra urna. Quisque fermentum est sed turpis consequat semper. Morbi suscipit orci vitae mauris ultricies interdum faucibus eros varius. Integer eget convallis orci. Morbi volutpat arcu eu mauris eleifend accumsan. Morbi porta placerat odio, eget posuere purus scelerisque vitae. Etiam urna leo, porttitor eu imperdiet ut, malesuada vitae justo. Donec tincidunt mauris sit amet dui venenatis facilisis. In est elit, gravida id posuere vitae, ultrices sit amet nisl. Integer neque nisi, lobortis a iaculis quis, sodales et dui. In sagittis tempor tristique. Quisque ut lacus ac massa malesuada commodo at ac ligula. Sed quis aliquet neque. Nam pretium orci et purus semper aliquet.</p>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque dignissim cursus risus vel interdum. Aenean eu augue erat, vel viverra urna. Quisque fermentum est sed turpis consequat semper. Morbi suscipit orci vitae mauris ultricies interdum faucibus eros varius. Integer eget convallis orci. Morbi volutpat arcu eu mauris eleifend accumsan. Morbi porta placerat odio, eget posuere purus scelerisque vitae. Etiam urna leo, porttitor eu imperdiet ut, malesuada vitae justo. Donec tincidunt mauris sit amet dui venenatis facilisis. In est elit, gravida id posuere vitae, ultrices sit amet nisl. Integer neque nisi, lobortis a iaculis quis, sodales et dui. In sagittis tempor tristique. Quisque ut lacus ac massa malesuada commodo at ac ligula. Sed quis aliquet neque. Nam pretium orci et purus semper aliquet.</p>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque dignissim cursus risus vel interdum. Aenean eu augue erat, vel viverra urna. Quisque fermentum est sed turpis consequat semper. Morbi suscipit orci vitae mauris ultricies interdum faucibus eros varius. Integer eget convallis orci. Morbi volutpat arcu eu mauris eleifend accumsan. Morbi porta placerat odio, eget posuere purus scelerisque vitae. Etiam urna leo, porttitor eu imperdiet ut, malesuada vitae justo. Donec tincidunt mauris sit amet dui venenatis facilisis. In est elit, gravida id posuere vitae, ultrices sit amet nisl. Integer neque nisi, lobortis a iaculis quis, sodales et dui. In sagittis tempor tristique. Quisque ut lacus ac massa malesuada commodo at ac ligula. Sed quis aliquet neque. Nam pretium orci et purus semper aliquet.</p>
    </div>
    </form>

    <script type="text/javascript">
        resizeSilverlight();

        window.onresize = resizeSilverlight;

    </script>
</body>
</html>