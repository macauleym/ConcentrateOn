using ConcentrateOn.Core.Enums;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Logic;
using ConcentrateOn.Core.Models;
using FluentAssertions;
using Moq;

namespace ConcentrateOn.Test.Logic;

public class ForLogicTests
{
    readonly Mock<IContextual<Subject>> subjectContextMock = new();
    readonly Mock<IContextual<Day>> daysContextMock        = new();

    [Fact]
    public void ParsesValidDayString()
    {
        // Arrange
        var day      = "Monday";
        var expected = DayOfWeek.Monday;

        var testTarget = new ForLogic(subjectContextMock.Object, daysContextMock.Object);
        
        // Act
        var actual = testTarget.ParsePossibleDayOfWeek(day);
        
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ParsesDayStringToRelativeDay()
    {
        // Arrange
        var day      = "tomorrow";
        var expected = DateTime.Now.AddDays(1).DayOfWeek;

        var testTarget = new ForLogic(subjectContextMock.Object, daysContextMock.Object);
        
        // Act
        var actual = testTarget.ParsePossibleDayOfWeek(day);
        
        // Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public async Task CanGetDayByName()
    {
        // Arrange
        var day      = DayOfWeek.Friday;
        var expected = new Day(Guid.Empty, day, []);

        daysContextMock.Setup(m => m
            .GetByAsync(day.ToString()))
            .ReturnsAsync(expected);
        
        var testTarget = new ForLogic(subjectContextMock.Object, daysContextMock.Object);
        
        // Act
        var actual = await testTarget.GetDayByNameAsync(day);
        
        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task CannotGetDayByNull()
    {
        // Arrange
        daysContextMock.Setup(m => m
            .GetByAsync(null))
            .ReturnsAsync((Day?)null);
        
        var testTarget = new ForLogic(subjectContextMock.Object, daysContextMock.Object);
        
        // Act
        var actual = await testTarget.GetDayByNameAsync(null);
        
        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task BuildsOutputStringOfSubjectsForDay()
    {
        // Arrange
        var subject1 = new Subject(Guid.NewGuid(), "Testing1", 1, During.Morning, "30m");
        var subject2 = new Subject(Guid.NewGuid(), "Testing2", 2, During.Night, "30m");
        var day      = new Day(Guid.NewGuid(), DayOfWeek.Tuesday, [subject1.Id, subject2.Id]);
        var expected =
            "*****************************\n"
            + "Desired Subjects for Tuesday\n"
            + "\t1. Testing1 - Morning, 30m\n"
            + "\t2. Testing2 - Night, 30m\n"
            + "*****************************\n";

        subjectContextMock.Setup(m => m
            .GetByAsync(subject1.Id))
            .ReturnsAsync(subject1);
        subjectContextMock.Setup(m => m
            .GetByAsync(subject2.Id))
            .ReturnsAsync(subject2);

        var testingTarget = new ForLogic(subjectContextMock.Object, daysContextMock.Object); 
        
        // Act
        var actual = await testingTarget.ComposeDaySubjectsStringAsync(day);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task BuildsOutputStringEvenWhenThereAreNoSubjects()
    {
        // Arrange
        var day      = new Day(Guid.NewGuid(), DayOfWeek.Tuesday, []);
        var expected =
            "*****************************\n"
            + "<< No subjects for this day >>\n"
            + "*****************************\n";
        
        var testingTarget = new ForLogic(subjectContextMock.Object, daysContextMock.Object); 
        
        // Act
        var actual = await testingTarget.ComposeDaySubjectsStringAsync(day);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
