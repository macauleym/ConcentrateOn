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
    public void CanGetExistingSubject()
    {
        // Arrange
        var name    = "Testing";
        var subject = new Subject(Guid.NewGuid(), name, 0, During.Afternoon, "5m");

        subjectContextMock.Setup(m => m
            .TryFind(name))
            .Returns(subject);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testTarget.TryGetSubject(name, out var result);

        // Assert
        actual.Should().BeTrue();
        result.Should().Be(subject);
    }
    
    [Fact]
    public void CantGetUnknownSubject()
    {
        // Arrange
        var name    = "blah";

        subjectContextMock.Setup(m => m
            .TryFind(name))
            .Returns((Subject?)null);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testTarget.TryGetSubject(name, out var result);

        // Assert
        actual.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void CanUpdateExistingSubject()
    {
        // Arrange
        var guid     = Guid.NewGuid();
        var request  = new OnRequest("unused", 2, During.Night, "2h", "Monday,Wednesday", false);
        var existing = new Subject(guid, "Testing", 0, During.Afternoon, "5m");
        var updated  = new Subject(guid, "Testing", 2, During.Night, "2h");

        subjectContextMock.Setup(m => m
            .Update(existing))
            .Returns(existing.Id);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testTarget.UpdateExistingSubject(existing, request);

        // Assert
        actual.Should().Be(existing.Id);
        subjectContextMock.Verify(context => context
            .Update(updated)
            , Times.Once
            );
    }
    
    [Fact]
    public void CanCreateNewSubject()
    {
        // Arrange
        var guid     = Guid.NewGuid();
        var request  = new OnRequest("unused", 2, During.Night, "2h", "Monday,Wednesday", false);
        var created  = new Subject(guid, "Testing", 2, During.Night, "2h");

        subjectContextMock.Setup(m => m
            .Create(It.IsAny<Subject>()))
            .Returns(created.Id);

        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testTarget.CreateNewSubject(request);

        // Assert
        actual.Should().NotBeEmpty();
        subjectContextMock.Verify(context => context
            .Create(It.IsAny<Subject>())
            , Times.Once
            );
    }

    [Fact]
    public void CanAssociateSubjectWithExistingDays()
    {
        // Arrange
        var targetId    = Guid.NewGuid();
        var mondayId    = Guid.NewGuid();
        var wednesdayId = Guid.NewGuid();
        var daysList    = new List<Day>
        { new (mondayId, DayOfWeek.Monday, [])
        , new (wednesdayId, DayOfWeek.Wednesday, [])
        };
        var expected    = new List<Day>
        { new (mondayId, DayOfWeek.Monday, [targetId])
        , new (wednesdayId, DayOfWeek.Wednesday, [targetId])
        };

        daysContextMock.Setup(m => m
            .All())
            .Returns(daysList);
        daysContextMock.Setup(m => m
            .TryFind(daysList[0].Name.ToString()))
            .Returns(daysList[0]);
        daysContextMock.Setup(m => m
            .TryFind(daysList[1].Name.ToString()))
            .Returns(daysList[1]);
        
        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testTarget.AssociateSubjectToDays(targetId, daysList.Select(d => d.Name).ToList());

        // Assert
        actual.Count.Should().Be(2);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void CanAssociateSubjectWithNewDays()
    { 
        // Arrange
        var targetId   = Guid.NewGuid();
        var daysList   = new List<Day>
        { new (Guid.NewGuid(), DayOfWeek.Monday, [])
        , new (Guid.NewGuid(), DayOfWeek.Wednesday, [])
        };

        daysContextMock.Setup(m => m
            .All())
            .Returns(daysList);
        daysContextMock.Setup(m => m
            .TryFind(daysList[0].Name.ToString()))
            .Returns(daysList[0]);
        daysContextMock.Setup(m => m
            .TryFind(daysList[1].Name.ToString()))
            .Returns(daysList[1]);
        daysContextMock.Setup(m => m
            .TryFind("Tuesday"))
            .Returns((Day?)null);
        
        var testTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testTarget.AssociateSubjectToDays(targetId, daysList.Select(d => d.Name).ToList());

        // Assert
        actual.Count.Should().Be(2);
        actual.Should().BeEquivalentTo(daysList);
    }
    
    [Fact]
    public void CanRemoveSubjectFromDays()
    {
        // Arrange
        var subjectId = Guid.NewGuid();
        var monday = new Day(Guid.NewGuid(), DayOfWeek.Monday, [subjectId]);
        var wednesday = new Day(Guid.NewGuid(), DayOfWeek.Wednesday, [subjectId]);
        var updatedDays = new List<Day> { monday };
        var allDays = new List<Day> { monday, wednesday };
        var expectedWednesday = wednesday with { SubjectIds = [] };
        var expected = new List<Day> { expectedWednesday };

        daysContextMock.Setup(m => m
            .All())
            .Returns(allDays);
        daysContextMock.Setup(m => m
            .Update(allDays[0]));
        daysContextMock.Setup(m => m
            .Update(allDays[1]));

        var testingTarget = new OnLogic(subjectContextMock.Object, daysContextMock.Object);

        // Act
        var actual = testingTarget.UnassociateUnwantedDays(subjectId, updatedDays);
        
        // Assert
        daysContextMock.Verify(context => context
            .Update(wednesday)
            , Times.Once);
        
        actual.Should().BeEquivalentTo(expected);
    }
}
