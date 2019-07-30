# LazyAsync.NET (.NET Standatd 2.0)
Fast and robust worker for blocking and none-blocking asynchronous operations
## Examples of usage
### Declaration
```
LazyAsync<DbData> lazyData = LazyAsync.Create(GetDbData);
```
### Implementaion 1
```
// lazyData automatically renews after 1 hour, data expires after 12 hours (blocking update)
private async Task<(DbData, DateTime renewAt, DateTime expiresAt)> GetDbData()
{
    DbData data;
    using (var ctx = dbFactory()) 
    {
        // getting data from db
    }
    return (data, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(12));
}
```
### Implementaion 2
```
private async Task<(DbData, DateTime renewAt, DateTime expiresAt)> GetDbData()
    => (await _someService.GetDbData(id), DateTime.UtcNow.AddSeconds(Ttl >> 1), DateTime.UtcNow.AddSeconds(Ttl))

```
### Usage

```
var data = await lazyData;
```