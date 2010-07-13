Pre-requisites:
1) Visual Studio 2008;
2) NUnit should be installed.

Procedure:
1) Check out the entire source from the trunk.
2) Open the file MnemonicFS.sln in Visual Studio.
3) Build the project.
4) The project may not build the first time; the reason for this may be that Sqlite DLLs may not be visible to the IDE. There are also issues between 32- and 64-bit platforms. In this case expand the References node in the Solution Explorer of the MnemonicFS project, remove the Sqlite entries and add them again using the Browse tab. Make sure you select the right Sqlite assemblies based on your platform type.
5) Open the file MnemonicFSTests.nunit in the NUnit runner. (If you've installed NUnit correctly, double-clicking the file should automatically open it in the runner.)
6) Press F5 to run all the tests.
7) Hopefully, you should see a green bar at the end of the test run. This may take some time depending on your system speed.
8) If you've reached this far, the library is ready for you to use.
