# iEdison-Cert-Tester
This is a very simplistic command line application to verify that a PKI certificate will correctly authenticate with the iEdison API from a Windows computer. It outputs many of the steps and values to command line for debugging.
This program requires you to have an active iEdison System Account with your PKI certificate uploaded to it.

There are 3 command line parameters that are required:
1. The full path and name to the PFX certificate file you would like to test. Example: "c:\testcert\mycert.pfx"
2. The password to the PFC file used in the first parameter. Example: MyPassword
3. The iEdison API URL to test against, either the live site or the UAT site. Example: "https://api-iedisonuat.nist.gov"

Or you can add the command line options directly to the launchSettings.json file contained in the project for easier testing.

The request made to the API is for the first invention with the invention title that contains the word 'test'.

The result is the raw JSON data returned from the iEdison API.

This was developed using Visual Studio 2022 and .NET 6.0.

This is presented with no warranty and no support.
