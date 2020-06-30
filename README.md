# Androsharp

An improved implementation of various ADB operations, notably `pull`. Androsharp contains an implementation of `pull` that
works with large files and/or faulty cables called `repull` (retry pull). 

# Download

[See the latest releases](https://github.com/Decimation/Androsharp/releases)

# Usage

Command line syntax:

`androsharp <command> [options...]`

- Angle brackets (`<>`) specify required arguments.

- Square brackets (`[]`) specify optional arguments. 

- Ellipses (`...`) specify one or more arguments.


# Commands

`repull <remote file> [block value] [block units] [destination file]`

Pulls the remote file from the device just like normal `adb pull` but with the Androsharp implementation.


# Explanation

ADB tends to hang/freeze when pulling large files (>3GB, it seems). Androsharp (`repull`) solves this problem 
by pulling blocks of data from the file and collating them to build the file. `repull` also continuously 
retries if it runs into errors.

# Inspiration

Most of this code was adapted from [this gist](https://gist.github.com/alopatindev/e94ff95ea834500abe2da81ac2a7764f) in Python.

# todo

- Refactor

- Performance improvements
