using ConcentrateOn.Core.Enums;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Logic;
using ConcentrateOn.Core.Models;
using FluentAssertions;
using Moq;

namespace ConcentrateOn.Test.Logic;

public class OnLogicTests
{
    readonly Mock<IContextual<Subject>> subjectContextMock = new();
    readonly Mock<IContextual<Day>> daysContextMock        = new();
    
    [Fact]
    public async Task CanGetExistingSubject()
    {
        // Arrange
        var name    = "Testing";
        var subject = new Subject(Guid.NewGuid(), name, 0, During.Afternoon, "5m");

        subjectContextMock.Setup(m => m
            .GetByAsync(name))
            .ReturnsAsync(subject);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = await testTarget.TryGetExistingAsync(name);

        // Assert
        actual.Item1.Should().BeTrue();
        actual.Item2.Should().Be(subject);
    }
    
    [Fact]
    public async Task CantGetUnknownSubject()
    {
        // Arrange
        var name    = "blah";

        subjectContextMock.Setup(m => m
            .GetByAsync(name))
            .ReturnsAsync((Subject?)null);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = await testTarget.TryGetExistingAsync(name);

        // Assert
        actual.Item1.Should().BeFalse();
        actual.Item2.Should().BeNull();
    }

    [Fact]
    public async Task CanUpdateExistingSubject()
    {
        // Arrange
        var guid     = Guid.NewGuid();
        var request  = new OnRequest("unused", 2, During.Night, "2h", "Monday,Wednesday");
        var existing = new Subject(guid, "Testing", 0, During.Afternoon, "5m");
        var updated  = new Subject(guid, "Testing", 2, During.Night, "2h");

        subjectContextMock.Setup(m => m
            .ResolveAsync(existing))
            .ReturnsAsync(existing.Id);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = await testTarget.UpdateExistingAsync(existing, request);

        // Assert
        actual.Should().Be(existing.Id);
        subjectContextMock.Verify(context => context
                .ResolveAsync(updated)
            , Times.Once
            );
    }
    
    [Fact]
    public async Task CanCreateNewSubject()
    {
        // Arrange
        var guid     = Guid.NewGuid();
        var request  = new OnRequest("unused", 2, During.Night, "2h", "Monday,Wednesday");
        var created  = new Subject(guid, "Testing", 2, During.Night, "2h");

        subjectContextMock.Setup(m => m
            .ResolveAsync(It.IsAny<Subject>()))
            .ReturnsAsync(created.Id);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = await testTarget.CreateNewAsync(request);

        // Assert
        actual.Should().NotBeEmpty();
        subjectContextMock.Verify(context => context
                .ResolveAsync(It.IsAny<Subject>())
            , Times.Once
            );
    }

    [Fact]
    public async Task CanAssociateSubjectWithExistingDays()
    {
        // Arrange
        var targetId   = Guid.NewGuid();
        var daysString = "Monday,Wednesday";
        var daysList   = new List<Day>
        { new (Guid.NewGuid(), DayOfWeek.Monday, [])
        , new (Guid.NewGuid(), DayOfWeek.Wednesday, [])
        };

        daysContextMock.Setup(m => m
            .GetAllAsync())
            .ReturnsAsync(daysList);
        daysContextMock.Setup(m => m
            .GetByAsync(daysList[0].Name.ToString()))
            .ReturnsAsync(daysList[0]);
        daysContextMock.Setup(m => m
            .GetByAsync(daysList[1].Name.ToString()))
            .ReturnsAsync(daysList[1]);
        
        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = await testTarget.AssociateDaysAsync(daysString, targetId);

        // Assert
        actual.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task CanAssociateSubjectWithNewDays()
    { 
        // Arrange
        var targetId   = Guid.NewGuid();
        var daysString = "Tuesday";
        var daysList   = new List<Day>
        { new (Guid.NewGuid(), DayOfWeek.Monday, [])
        , new (Guid.NewGuid(), DayOfWeek.Wednesday, [])
        };

        daysContextMock.Setup(m => m
            .GetAllAsync())
            .ReturnsAsync(daysList);
        daysContextMock.Setup(m => m
            .GetByAsync(daysList[0].Name.ToString()))
            .ReturnsAsync(daysList[0]);
        daysContextMock.Setup(m => m
            .GetByAsync(daysList[1].Name.ToString()))
            .ReturnsAsync(daysList[1]);
        daysContextMock.Setup(m => m
            .GetByAsync("Tuesday"))
            .ReturnsAsync((Day?)null);
        
        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = await testTarget.AssociateDaysAsync(daysString, targetId);

        // Assert
        actual.Count.Should().Be(2);
        actual.Should().BeEquivalentTo(daysList);
    }
    
    [Fact]
    public async Task CanRemoveSubjectFromDays()
    {
        // Arrange
        var subjectId = Guid.NewGuid();
        var complimentDays = new List<Day>
        { new (Guid.NewGuid(), DayOfWeek.Monday, [subjectId])
        , new (Guid.NewGuid(), DayOfWeek.Wednesday, [subjectId])
        };
        var expectedMonday = complimentDays[0];
        expectedMonday.SubjectIds.Clear();
        var expectedWednesday = complimentDays[1];
        expectedWednesday.SubjectIds.Clear();

        daysContextMock.Setup(m => m
            .ResolveAsync(complimentDays[0]));
        daysContextMock.Setup(m => m
            .ResolveAsync(complimentDays[1]));

        var testingTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        await testingTarget.RemoveUnwantedDaysAsync(complimentDays, subjectId);
        
        // Assert
        daysContextMock.Verify(context => context
                .ResolveAsync(expectedMonday)
            , Times.Once);
        daysContextMock.Verify(context => context
                .ResolveAsync(expectedWednesday)
            , Times.Once);
    }
}
