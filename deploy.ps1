#####################################################################################################################
#
# This PowerShell script simplifies deploying this solution to AWS Lambda using different environments
#
# Usage:
#	1. Go to Tools > Command Line > Developer PowerShell 
#	   OR open a PowerShell window and 'cd' to the root directory of the solution 
#	   where deploy.ps1 is located (ex. C:\VS\mfc-lambda-api\)
#   2. Type in .\deploy.ps1
#   3. Hit enter
#   4. Script will open in new PowerShell window, follow prompts
#
#####################################################################################################################

$option = Read-Host -Prompt '
Select the deployment stage to target (ex: 2): 

	1. Development
	2. Staging
	3. Production
	
Selection'

$stageName = ""

switch ($option)
{
    1 {$stageName = "development"}
    2 {$stageName = "staging"}
    3 {$stageName = "production"}
}

$configFilePath = "Deployment\deploy.configs.$stageName.json"

$json = Get-Content -Raw -Path "MyHttpGatewayApi\Deployment\deploy.configs.$stageName.json" | ConvertFrom-Json

$display = $json | Format-List | Out-String

$continue = Read-Host -Prompt "
This will deploy this lambda function with the following settings: $display Do you want to proceed? (y/n)"

if ($continue -eq 'y' -OR $continue -eq 'Y') {
	$argList = "dotnet lambda deploy-function -C $stagename -cfg $configFilePath -pcfg true -pl MyHttpGatewayApi\ --aws-access-key-id AKIAQRRFDAFZUHPDOEGG --aws-secret-key 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"
	
	Start-Process -FilePath PowerShell -ArgumentList $argList -Wait
	
	if ($stageName -ne "production") {
		Write-Host "
		If there were no errors you can access this API from https://$stageName.api.myfleet.services
		"
	} else {
		Write-Host "
		If there were no errors you can access this API from https://api.myfleet.services
		"
	}
}
