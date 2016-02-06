genX.sln Solution

This solution is intended to be used to build and package genX for
delivery.  It contains the class library, sample applications, setup
project, and a utility project (Build) that executes the necessary
build scripts.

The following process occurs to build a package:

1. genX.DLL is built
2. genX.DLL is copied to the solution root directory.  This allows
   the sample applications to contain relative paths to genX.DLL.
3. Sample applications are built.
4. Help files are compiled [this is not currently done automatically].
5. Setup project is built.

The output is a genX.msi installer package.

OK, that's what should happen.  For now, the following has to happen:

1. Build the project as normal,
2. Build the documentation manually using HTML Help Workshop,
3. Build the API documentation manually using NDoc,
4. Come back and do a build of the Setup project.

