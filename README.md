Scale-Storage
=============

Simple Azure Table, Queue and Blob Storage wrappers for .NET. These are mostly based on the MSDN guidance articles, including:

* [How to use Blob storage from .NET](http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/)

Library is .NET4.5 and Async by default.

## `AzureBlobStorage`
Azure Block Blob Storage library wrapper for .NET. Only [Block Storage] is currently supported. 

### `List()`
Gets a list of `CloudBlob` for a given `containerName` and optional `folderName`.

**Params**
* `containerName`: The name of the container.
* `folderName`: The folder name of the Blobs to list.
* `flatten`: When true, will return a flattened list of all Blobs in the container and sub-folders.
* `pages`: The maximum number of Pages of items to retrieve. There are 5000 items per Page (this is the API limit). Default is 1.

Returns `Task<IEnumerable<CloudBlob>>`. Streams are not set on these results, use `Download()` to get a Stream.


[Block Storage]:https://msdn.microsoft.com/en-us/library/azure/ee691964.aspx
