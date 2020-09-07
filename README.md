# ItsaMultiCopy!

Copies the entire contents of a directory from one location to another.

## About

This is a tool I wroted becuase I needed it and there was nothing to use.

Batch and PowerShell's internal copy commands do not have a proper recurse
option like Unix `cp -r`.

Robocopy has incomprehensible DOS syntax and *returns 1 on success*.

WSL commands are difficult to invoke from a Windows context like Visual
Studio build events.

## Options

    ItsaMultiCopy.exe OPTIONS SOURCE DESTINATION

    -n, --no-recurse        If specified, will not recurse. (Subdirectories of the
                            source directory will not be copied.)

    -d, --data              If specified, will only copy the binary file data and
                            no ACLs or alternate streams.

    -c, --changed           If specified, will not copy files with a older
                            last-modified time.

    -v, --verbose           If specified, will print the name of each file copied.

    -m, --matches           If specified, will only copy files matching the glob
                            pattern.

    -M, --subdir-matches    If specified, will only copy subdirectories matching
                            the glob pattern.

    --help                  Display this help screen.
    
    --version               Display version information.

## Return codes

    0       Success
    1       An error occured while copying files.
    2       Invalid command line arguments.

## Copyright

Copyright © 2020 Miff

Licensed under the GPL version 3, see LICENSE.txt for details.
