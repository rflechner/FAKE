# Publish Website with MSDeploy

## Prerequisite

You have to install MS Delpoy on your IIS server.
A good tutorial is available here: 

https://www.iis.net/learn/install/installing-publishing-technologies/installing-and-configuring-web-deploy-on-iis-80-or-later

## Usage example:

An example of target using this helper could be:

    open Fake
    open MSDeployHelper

    let basePath p =
        __SOURCE_DIRECTORY__ @@ p

    Target "DeployWebService" (fun _ ->
      let projectDir = basePath "ProjectName"
      let packageFullPath = buildDir @@ "ProjectName.zip"
      MSDeployCreatePackage (fun p -> { p with ProjectDir = projectDir; PackageFullPath = packageFullPath})
    )

    Target "InstallWebSite" (fun _ ->
      let projectDir = "mydir ..."
      let packageFullPath = buildDir @@ "msdeploy_package.zip"
      trace "Installing website"
      printfn "Please type username"
      let username = Console.ReadLine()
      printfn "Please type password"
      let password = Console.ReadLine()
      MSDeployInstallPackage
        <| fun p -> 
             { p 
                with 
                  PackageFullPath=packageFullPath
                  UserName=username
                  Password=password
                  AllowUntrusted=true
                  IISAppName="test_deploy"
                  ServerMsDeployUri="https://xxx.xxx.xxx.xxx:8172/MsDeploy.axd" //default uri with your IP
             }
    )



