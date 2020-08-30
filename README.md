# XUnitAsyncExample
  
**Problem description:**  
When using localdb in Windows 10, async unit tests will fail even if the code it's testing works.  In some circumstances, as with the project here, the tests when run as a group all seem to work fine.  When running the async unit tests (that have database calls to Open) individually or if they execute before the non-async tests (that have database calls to Open), they will throw an exception.  If the connection string is repointed to a real SQL Server instance, the problem disappears.

Here are the bits of information I have based on the testing I've done to try and isolate the issue:

- When the unit tests in this project are run together, they succeed (possibly due to the non-async elements executing before the async ones run).  
- When the async tests are run individually they cause an exception.   
- The exception occurs when connection.Open() is called in the code under test.  
- Non-async tests succeed regardless of how they are run (together or individually).  
- Running selected async and non-async unit tests having database calls (when non-async executes before the async tests) pass.  
- If you create a console app and run the code, it works fine.  It only seems to be when executing under the test runner.  

Other things that have been tried...  Installed Polly from nuget and used the Handle<Exception> and Retry with a large number of retries, still fails after exhausting the 
retries.  Tried a suggestion from a reported NUnit problem on older Windows that suggested removing the Integrated Security = True from the connection string,
but that didn't make any difference.  I tried it in JetBrains Rider, but I had to move over to .NET Core as Framework isn't supported in Rider.  I wasn't able to get it to fail,
but it's not an apples-to-apples comparison.

Here's the version bits...  
Windows 10 Enterprise 1909  
VS2017 15.9.26  
VS2019 16.7.2  
XUnit.net 2.4.1  
JustMock 2020.2.616.1 (free version)  
System.Data.SqlClient  
localdb (13.0.4001 - equivalently 2016 SP1)  
SQL Server 2016 SP2 CU14  
ReSharper 2020.2.1  



Stack trace when running ExecuteSelectStatementScalarAsync_Correct_Equals1() by itself:
```
System.ApplicationException
DB.DBManager+<ExecuteSelectStatementScalarAsync>d__15.MoveNext() - Exception thrown
   at DB.DBManager.<ExecuteSelectStatementScalarAsync>d__15.MoveNext() in C:\Users\dtaylor\source\repos\XUnitAsyncExample\Example\DBManager.cs:line 363
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at DBTest.DBManagerTest.<ExecuteSelectStatementScalarAsync_Correct_Equals1>d__10.MoveNext() in C:\Users\dtaylor\source\repos\XUnitAsyncExample\Example\DBManagerTest.cs:line 77
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at Xunit.Sdk.TestInvoker`1.<>c__DisplayClass48_1.<<InvokeTestMethodAsync>b__1>d.MoveNext() in C:\Dev\xunit\xunit\src\xunit.execution\Sdk\Frameworks\Runners\TestInvoker.cs:line 264
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at Xunit.Sdk.ExecutionTimer.<AggregateAsync>d__4.MoveNext() in C:\Dev\xunit\xunit\src\xunit.execution\Sdk\Frameworks\ExecutionTimer.cs:line 48
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at Xunit.Sdk.ExceptionAggregator.<RunAsync>d__9.MoveNext() in C:\Dev\xunit\xunit\src\xunit.core\Sdk\ExceptionAggregator.cs:line 90

System.IO.FileNotFoundException
Could not load file or assembly 'Example, Version=1.0.2.1843, Culture=neutral, PublicKeyToken=null' or one of its dependencies. The system cannot find the file specified.
   at System.Reflection.RuntimeAssembly._nLoad(AssemblyName fileName, String codeBase, Evidence assemblySecurity, RuntimeAssembly locationHint, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.InternalLoad(String assemblyString, Evidence assemblySecurity, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean forIntrospection)
   at System.Reflection.RuntimeAssembly.InternalLoad(String assemblyString, Evidence assemblySecurity, StackCrawlMark& stackMark, Boolean forIntrospection)
   at System.Reflection.Assembly.Load(String assemblyString)
   at System.Runtime.Serialization.FormatterServices.LoadAssemblyFromString(String assemblyName)
   at System.Reflection.MemberInfoSerializationHolder..ctor(SerializationInfo info, StreamingContext context)
   at System.AppDomain.GetHostEvidence(Type type)
   at System.Security.Policy.Evidence.GetHostEvidenceNoLock(Type type)
   at System.Security.Policy.Evidence.RawEvidenceEnumerator.MoveNext()
   at System.Security.Policy.Evidence.EvidenceEnumerator.MoveNext()
   at System.Configuration.ClientConfigPaths.GetEvidenceInfo(AppDomain appDomain, String exePath, String& typeName)
   at System.Configuration.ClientConfigPaths.GetTypeAndHashSuffix(AppDomain appDomain, String exePath)
   at System.Configuration.ClientConfigPaths..ctor(String exePath, Boolean includeUserConfig)
   at System.Configuration.ClientConfigPaths.GetPaths(String exePath, Boolean includeUserConfig)
   at System.Configuration.ClientConfigurationHost.get_ConfigPaths()
   at System.Configuration.ClientConfigurationHost.RequireCompleteInit(IInternalConfigRecord record)
   at System.Configuration.BaseConfigurationRecord.GetSectionRecursive(String configKey, Boolean getLkg, Boolean checkPermission, Boolean getRuntimeObject, Boolean requestIsHere, Object& result, Object& resultRuntimeObject)
   at System.Configuration.BaseConfigurationRecord.GetSection(String configKey, Boolean getLkg, Boolean checkPermission)
   at System.Configuration.ConfigurationManager.GetSection(String sectionName)
   at System.Configuration.PrivilegedConfigurationManager.GetSection(String sectionName)
   at System.Data.LocalDBAPI.CreateLocalDBInstance(String instance)
   at System.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, Boolean ignoreSniOpenTimeout, Int64 timerExpire, Boolean encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean withFailover, Boolean isFirstTransparentAttempt, SqlAuthenticationMethod authType, Boolean disableTnir, SqlAuthenticationProviderManager sqlAuthProviderManager)
   at System.Data.SqlClient.SqlInternalConnectionTds.AttemptOneLogin(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean ignoreSniOpenTimeout, TimeoutTimer timeout, Boolean withFailover, Boolean isFirstTransparentAttempt, Boolean disableTnir)
   at System.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at System.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at System.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, DbConnectionPool pool, String accessToken, Boolean applyTransientFaultHandling, SqlAuthenticationProviderManager sqlAuthProviderManager)
   at System.Data.SqlClient.SqlConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionFactory.CreatePooledConnection(DbConnectionPool pool, DbConnection owningObject, DbConnectionOptions options, DbConnectionPoolKey poolKey, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionPool.CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at System.Data.ProviderBase.DbConnectionPool.UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, UInt32 waitForMultipleObjectsTimeout, Boolean allowCreate, Boolean onlyOneCheckConnection, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionFactory.TryGetConnection(DbConnection owningConnection, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionInternal.TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   at System.Data.SqlClient.SqlConnection.TryOpenInner(TaskCompletionSource`1 retry)
   at System.Data.SqlClient.SqlConnection.TryOpen(TaskCompletionSource`1 retry)
   at System.Data.SqlClient.SqlConnection.Open()
   at DB.DBManager.<ExecuteSelectStatementScalarAsync>d__15.MoveNext() in C:\Users\dtaylor\source\repos\XUnitAsyncExample\Example\DBManager.cs:line 347

```

Stack trace when running ExecuteSelectStatementScalarAsync_Correct_Equals1() by itself:
```
System.ApplicationException
DB.DBManager+<ExecuteSelectStatementScalarAsync>d__15.MoveNext() - Exception thrown
   at DB.DBManager.<ExecuteSelectStatementScalarAsync>d__15.MoveNext() in C:\Users\dtaylor\source\repos\XUnitAsyncExample\Example\DBManager.cs:line 363
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at DBTest.DBManagerTest.<ExecuteSelectStatementScalarAsync_Correct_Equals1>d__10.MoveNext() in C:\Users\dtaylor\source\repos\XUnitAsyncExample\Example\DBManagerTest.cs:line 77
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at Xunit.Sdk.TestInvoker`1.<>c__DisplayClass48_1.<<InvokeTestMethodAsync>b__1>d.MoveNext() in C:\Dev\xunit\xunit\src\xunit.execution\Sdk\Frameworks\Runners\TestInvoker.cs:line 264
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at Xunit.Sdk.ExecutionTimer.<AggregateAsync>d__4.MoveNext() in C:\Dev\xunit\xunit\src\xunit.execution\Sdk\Frameworks\ExecutionTimer.cs:line 48
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd(Task task)
   at Xunit.Sdk.ExceptionAggregator.<RunAsync>d__9.MoveNext() in C:\Dev\xunit\xunit\src\xunit.core\Sdk\ExceptionAggregator.cs:line 90

System.IO.FileNotFoundException
Could not load file or assembly 'Example, Version=1.0.2.1843, Culture=neutral, PublicKeyToken=null' or one of its dependencies. The system cannot find the file specified.
   at System.Reflection.RuntimeAssembly._nLoad(AssemblyName fileName, String codeBase, Evidence assemblySecurity, RuntimeAssembly locationHint, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.InternalLoad(String assemblyString, Evidence assemblySecurity, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean forIntrospection)
   at System.Reflection.RuntimeAssembly.InternalLoad(String assemblyString, Evidence assemblySecurity, StackCrawlMark& stackMark, Boolean forIntrospection)
   at System.Reflection.Assembly.Load(String assemblyString)
   at System.Runtime.Serialization.FormatterServices.LoadAssemblyFromString(String assemblyName)
   at System.Reflection.MemberInfoSerializationHolder..ctor(SerializationInfo info, StreamingContext context)
   at System.AppDomain.GetHostEvidence(Type type)
   at System.Security.Policy.Evidence.GetHostEvidenceNoLock(Type type)
   at System.Security.Policy.Evidence.RawEvidenceEnumerator.MoveNext()
   at System.Security.Policy.Evidence.EvidenceEnumerator.MoveNext()
   at System.Configuration.ClientConfigPaths.GetEvidenceInfo(AppDomain appDomain, String exePath, String& typeName)
   at System.Configuration.ClientConfigPaths.GetTypeAndHashSuffix(AppDomain appDomain, String exePath)
   at System.Configuration.ClientConfigPaths..ctor(String exePath, Boolean includeUserConfig)
   at System.Configuration.ClientConfigPaths.GetPaths(String exePath, Boolean includeUserConfig)
   at System.Configuration.ClientConfigurationHost.get_ConfigPaths()
   at System.Configuration.ClientConfigurationHost.RequireCompleteInit(IInternalConfigRecord record)
   at System.Configuration.BaseConfigurationRecord.GetSectionRecursive(String configKey, Boolean getLkg, Boolean checkPermission, Boolean getRuntimeObject, Boolean requestIsHere, Object& result, Object& resultRuntimeObject)
   at System.Configuration.BaseConfigurationRecord.GetSection(String configKey, Boolean getLkg, Boolean checkPermission)
   at System.Configuration.ConfigurationManager.GetSection(String sectionName)
   at System.Configuration.PrivilegedConfigurationManager.GetSection(String sectionName)
   at System.Data.LocalDBAPI.CreateLocalDBInstance(String instance)
   at System.Data.SqlClient.TdsParser.Connect(ServerInfo serverInfo, SqlInternalConnectionTds connHandler, Boolean ignoreSniOpenTimeout, Int64 timerExpire, Boolean encrypt, Boolean trustServerCert, Boolean integratedSecurity, Boolean withFailover, Boolean isFirstTransparentAttempt, SqlAuthenticationMethod authType, Boolean disableTnir, SqlAuthenticationProviderManager sqlAuthProviderManager)
   at System.Data.SqlClient.SqlInternalConnectionTds.AttemptOneLogin(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean ignoreSniOpenTimeout, TimeoutTimer timeout, Boolean withFailover, Boolean isFirstTransparentAttempt, Boolean disableTnir)
   at System.Data.SqlClient.SqlInternalConnectionTds.LoginNoFailover(ServerInfo serverInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString connectionOptions, SqlCredential credential, TimeoutTimer timeout)
   at System.Data.SqlClient.SqlInternalConnectionTds.OpenLoginEnlist(TimeoutTimer timeout, SqlConnectionString connectionOptions, SqlCredential credential, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance)
   at System.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, DbConnectionPool pool, String accessToken, Boolean applyTransientFaultHandling, SqlAuthenticationProviderManager sqlAuthProviderManager)
   at System.Data.SqlClient.SqlConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionFactory.CreatePooledConnection(DbConnectionPool pool, DbConnection owningObject, DbConnectionOptions options, DbConnectionPoolKey poolKey, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionPool.CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at System.Data.ProviderBase.DbConnectionPool.UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, UInt32 waitForMultipleObjectsTimeout, Boolean allowCreate, Boolean onlyOneCheckConnection, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionFactory.TryGetConnection(DbConnection owningConnection, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionInternal.TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   at System.Data.SqlClient.SqlConnection.TryOpenInner(TaskCompletionSource`1 retry)
   at System.Data.SqlClient.SqlConnection.TryOpen(TaskCompletionSource`1 retry)
   at System.Data.SqlClient.SqlConnection.Open()
   at DB.DBManager.<ExecuteSelectStatementScalarAsync>d__15.MoveNext() in C:\Users\dtaylor\source\repos\XUnitAsyncExample\Example\DBManager.cs:line 347

```

