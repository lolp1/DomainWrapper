# DomainWrapper
### DomainWrapper is set of classes to provide an easy way to handle the injection of .net code into a process.

* Simply inject the dll and call the export below with the path being replaced with the applications path you want to host.
* Example: LoadDomainHostSettings(C:\Users\Name\Desktop\Test\TestApp.exe)
```csharp
LoadDomainHostSettings(string applicationPath)
```

## Credits
*miceiken/Scorpiona deserve all credits for the base domain classes.
