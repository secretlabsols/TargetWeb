
The Security web service is actually defined in the Target.Web.Apps project so it can be used from other web projects that do not
reference Target.Web.  The Security.asmx file is just a placeholder so the web service can be accessed.  
The CodeBehind attribute is explicitly removed from this file.