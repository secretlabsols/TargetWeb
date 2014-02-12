
These .browser files (with the exception of "default.browser") are taken from http://owenbrady.net/browsercaps/ (the .NET v2/v3 definitions).
Note that the "OceanSpiders.browser" file is explicitly not included because it causes the following error to occur and renders the site useless:



Server Error in '/TargetWeb' Application.
--------------------------------------------------------------------------------
Parser Error 
Description: An error occurred during the parsing of a resource required to service this request. Please review the following specific parse error details and modify your source file appropriately. 

Parser Error Message: Object reference not set to an instance of an object.

Source Error: 
[No relevant source lines]

Source File: none    Line: 1 
--------------------------------------------------------------------------------
Version Information: Microsoft .NET Framework Version:2.0.50727.3615; ASP.NET Version:2.0.50727.3614 




Enabling debugging of .NET Framework source allows the following extra information to be gleaned but the actual source is still unreachable.
Subsequent attempts to track down the cause have been unsuccessful.

System.Web.HttpParseException occurred
  Message="Object reference not set to an instance of an object."
  Source="System.Web"
  ErrorCode=-2147467259
  Line=1
  StackTrace:
       at System.Web.Compilation.AssemblyBuilder.AddBuildProvider(BuildProvider buildProvider)
  InnerException: System.NullReferenceException
       Message="Object reference not set to an instance of an object."
       Source="System.Web"
       StackTrace:
            at System.Web.Compilation.ApplicationBrowserCapabilitiesCodeGenerator.GenerateCode(AssemblyBuilder assemblyBuilder)
            at System.Web.Compilation.ApplicationBrowserCapabilitiesBuildProvider.GenerateCode(AssemblyBuilder assemblyBuilder)
            at System.Web.Compilation.AssemblyBuilder.AddBuildProvider(BuildProvider buildProvider)
       InnerException: 
