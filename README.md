# XUnitAsyncExample
XUnit.net Async Problem Example

When the unit tests are run together, they succeed.  When the async tests are run separately they cause an exception.  
The exception typically occurs when connection.Open() is called in the code under test.

Non-async tests succeed regardless of how they are run.

