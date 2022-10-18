GetProjectDependencies
======

Determine the full list of all the referenced libraries for a given project (csproj)

# Usage:

```bash
dotnet run <csproj full path> [<root directory to trim>]
```


# For example:

## Example 1
```
dotnet run "/some/root/path/test/proj1/proj1.csproj"
``` 
will output 
```
/some/root/path/test/proj1/*
```

## Example 2:

```
dotnet run "/some/root/path/test/proj1/proj1.csproj" "/some/root/path/test/"
```
will output
```
proj1/*
```



# Other

Towards creating the test project structure:
- create console project `dotnet new console -n proj1`
- create class lib project `dotnet new classlib -n lib1`
- add project reference `dotnet add reference ../lib3/lib3.csproj`