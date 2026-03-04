# BoardService Tests

## Overview

Comprehensive unit tests for the `BoardService` class, covering all CRUD operations, member management, and authorization scenarios.

## Test Framework

- **xUnit** - Testing framework
- **Entity Framework Core InMemory** - In-memory database for testing
- **.NET 9.0** - Test project target framework

## Test Results

```
? 32/32 tests passing (100% success rate)
```

## Test Coverage

### By Feature Area

| Feature | Tests | Status |
|---------|-------|--------|
| Board Creation | 2 | ? Passing |
| Board Retrieval | 5 | ? Passing |
| Board Update | 4 | ? Passing |
| Board Deletion | 3 | ? Passing |
| Member Management | 10 | ? Passing |
| Authorization Checks | 8 | ? Passing |

### Authorization Scenarios Tested

- ? Only board members can view board details
- ? Only board owners can update board name
- ? Only board owners can delete boards
- ? Only board owners can add/remove/update members
- ? Cannot remove the last owner from a board
- ? Users can only view their own board lists

## Running the Tests

### Run all tests
```bash
dotnet test KanbanAPI.Tests
```

### Run specific test class
```bash
dotnet test --filter "FullyQualifiedName~BoardServiceTests"
```

### Run with detailed output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Structure

Each test follows the **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and context
    var board = await CreateTestBoardWithMember(_testUserId, BoardRole.Owner);
    
    // Act - Execute the method being tested
    var result = await _boardService.DeleteAsync(board.Id, _testUserId);
    
    // Assert - Verify the expected outcome
    Assert.True(result);
}
```

## Helper Methods

- `CreateTestBoardWithMember(userId, role)` - Creates a board with a specific user as member
- `AddMemberToBoard(boardId, userId, role)` - Adds an additional member to an existing board

## Key Testing Insights

1. **InMemory Database Isolation**: Each test uses a unique InMemory database instance via `Guid.NewGuid().ToString()` as database name
2. **User Setup**: Test users are added in the constructor to support EF Core navigation property loading
3. **Authorization Focus**: Majority of tests verify authorization rules are properly enforced
4. **Edge Cases**: Tests cover scenarios like removing the last owner, duplicate members, non-existent boards, etc.

## Dependencies

- `xunit` - 2.8.2+
- `Microsoft.EntityFrameworkCore.InMemory` - 8.0.23
- `KanbanAPI` - Project reference

## Future Test Enhancements

- Integration tests with real database
- Performance tests for large datasets
- Concurrent access scenarios
- Additional edge case coverage
