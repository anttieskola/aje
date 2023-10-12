# Exceptions
Some notes of exception usage in this app

# Customs
- PlatformException: kill yourself
- ParsingException: Error reading data into platform
- KeyExistsException: kill yourself

# Framework ones
- ValidationException: Data validation fails
- DataException: Data handling fails (ignore the origin of ado.net)

- ArgumentException: A non-null argument that is passed to a method is invalid.
- ArgumentNullException: An argument that is passed to a method is null.
- ArgumentOutOfRangeException: An argument is outside the range of valid values.
- DirectoryNotFoundException: Part of a directory path is not valid.
- DivideByZeroException: The denominator in an integer or Decimal division operation is zero.
- DriveNotFoundException: A drive is unavailable or does not exist.
- FileNotFoundException: A file does not exist.
- FormatException: A value is not in an appropriate format to be converted from a string by a conversion method such as Parse.
- IndexOutOfRangeException: An index is outside the bounds of an array or collection.
- InvalidOperationException: A method call is invalid in an object's current state.
- KeyNotFoundException: The specified key for accessing a member in a collection cannot be found.
- NotImplementedException: A method or operation is not implemented.
- NotSupportedException: A method or operation is not supported.
- ObjectDisposedException: An operation is performed on an object that has been disposed.
- OverflowException: An arithmetic, casting, or conversion operation results in an overflow.
- PathTooLongException: A path or file name exceeds the maximum system-defined length.
- PlatformNotSupportedException: The operation is not supported on the current platform.
- RankException: An array with the wrong number of dimensions is passed to a method.
- TimeoutException: The time interval allotted to an operation has expired.
- UriFormatException: An invalid Uniform Resource Identifier (URI) is used.

## Links
[Exceptions](https://learn.microsoft.com/en-us/dotnet/api/system.exception?view=net-7.0)
[Framework guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/using-standard-exception-types)
