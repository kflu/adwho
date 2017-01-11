# adwho
A command line tool (and library) to conveniently query AD.

## Features

- A single executable binary, no external dependencies.
- Output JSON format, easy to automate.

## TODOs

Those are the features that's totally doable but I'm not sure how useful they are given my limited time. Let me know if you need any of those (or better send a PR) by opening a issue.

- More control over authentication. Right now it only uses your default logged on credential to access the default AD server.


## Install

Get from [Github release](https://github.com/kflu/adwho/releases).

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

## Examples

Query directory entry that has `name` match `John Doe`:

    $ adwho name "John Doe"
    ... this prints out the result in JSON format

Get the `name` of the entry with specified SMTP address:

    $ adwho smtp "foo@example.com" -p name
    "Foo bar"
