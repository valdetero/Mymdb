using System;
using System.Reflection;

public interface IMethodDecorator
{
    void OnEntry();
    void OnExit();
    void OnException(Exception exception);
}
