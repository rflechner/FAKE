module Fake.MSDeployHelper

open Fake
open ProcessHelper
open System
open System.IO

type MSDeployPackageParams = 
  { ProjectDir:string
    PackageFullPath:string
    Timeout:TimeSpan }
  static member Default = 
    { ProjectDir=""; PackageFullPath=""; Timeout=TimeSpan.FromMinutes 2. }
type MSDeployInstallParams = 
  { IISAppName:string
    PackageFullPath:string
    ServerMsDeployUri:string
    UserName:string
    Password:string
    Timeout:TimeSpan
    AllowUntrusted:bool }
  static member Default = 
    { IISAppName=""; PackageFullPath=""; Timeout=(TimeSpan.FromMinutes 2.); 
      ServerMsDeployUri="https://localhost:8172/MsDeploy.axd"; UserName="";
      Password=""; AllowUntrusted=false }

let ExecMSDeploy timeout args =
  let exe = "msdeploy.exe"
  let result =
    ExecProcess
      (fun info ->
          info.FileName <- exe
          info.Arguments <- args) timeout
  if result <> 0
  then failwithf "Process '%s %s' failed with exit code '%d'" exe args result

let MSDeployCreatePackage (f: MSDeployPackageParams -> MSDeployPackageParams) =
  let p = f MSDeployPackageParams.Default
  let args = sprintf """-verb:sync -source:iisApp="%s" -dest:package="%s" """ p.ProjectDir p.PackageFullPath
  ExecMSDeploy p.Timeout args

let MSDeployInstallPackage (f: MSDeployInstallParams -> MSDeployInstallParams) =
  let p = f MSDeployInstallParams.Default
  let args = 
    sprintf 
      """-verb:sync -source:package="%s" -dest:iisApp="%s",ComputerName="%s",UserName='%s',Password='%s',AuthType='Basic' """ 
        p.PackageFullPath p.IISAppName p.ServerMsDeployUri p.UserName p.Password
  match p.AllowUntrusted with
  | true -> args + " -allowUntrusted"
  | false -> args
  |> ExecMSDeploy p.Timeout
