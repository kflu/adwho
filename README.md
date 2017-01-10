# adwho
A command line tool (and library) to conveniently query AD.

## Usage

    USAGE: adwho.exe [--help] [--prop <prop>] [--multiple] [--searchroot <root>] <search> <string>

    SEARCH:

        <search> <string>     Search based on key value

    OPTIONS:

        --prop, -p <prop>     Optional property to get once a search result is returned. If omitted, program write the
                            search result in JSON format
        --multiple, -m        Returns multiple results in JSON format
        --searchroot, -r <root>
                            Search root, example: LDAP://OU=UserAccounts,DC=foo,DC=bar,DC=baidu,DC=com
        --help                display this list of options.

