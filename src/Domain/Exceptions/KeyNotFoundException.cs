﻿namespace AJE.Domain.Exceptions;

public class KeyNotFoundException : Exception
{
    public KeyNotFoundException(string? message) : base(message)
    {
    }
}
