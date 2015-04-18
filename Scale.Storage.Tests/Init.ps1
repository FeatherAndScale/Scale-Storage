param( 
         [parameter(Position=0,Mandatory=$true)] 
         [String]$AzureConnectionString 
     )

Write-Host 'Setting env:AZURE_STORAGE_CONNECTION_STRING. Restart Visual Studio and this command window for the new Envrionment Variable to take effect.'
[Environment]::SetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING", $AzureConnectionString, "User")

Write-Host 'Finished.'