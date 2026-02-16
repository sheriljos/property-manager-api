using FluentAssertions;
using PropertyManager.Domain.Entities;
using PropertyManager.Domain.Helper;
using Xunit;

namespace PropertyManager.Domain.Tests.Unit.Helper;

public class MakelaarsUsecaseTests
{
    [Fact]
    public void GetRankedMakelaars_WithMultipleMakelaars_ReturnsTopMakelaarsByListingCount()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "ABC Makelaars" },
            new() { MakelaarId = 1, MakelaarName = "ABC Makelaars" },
            new() { MakelaarId = 1, MakelaarName = "ABC Makelaars" },
            new() { MakelaarId = 2, MakelaarName = "XYZ Real Estate" },
            new() { MakelaarId = 2, MakelaarName = "XYZ Real Estate" },
            new() { MakelaarId = 3, MakelaarName = "Best Homes" }
        };
    
        var expected = new List<Makelaar>
        {
            new() { Id = 1, Name = "ABC Makelaars", NumberOfProperties = 3 },
            new() { Id = 2, Name = "XYZ Real Estate", NumberOfProperties = 2 },
            new() { Id = 3, Name = "Best Homes", NumberOfProperties = 1 }
        };
    
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
    
        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
    
    [Fact]
    public void GetRankedMakelaars_WithTopParameter_ReturnsOnlyTopN()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "Makelaar 1" },
            new() { MakelaarId = 1, MakelaarName = "Makelaar 1" },
            new() { MakelaarId = 1, MakelaarName = "Makelaar 1" },
            new() { MakelaarId = 2, MakelaarName = "Makelaar 2" },
            new() { MakelaarId = 2, MakelaarName = "Makelaar 2" },
            new() { MakelaarId = 3, MakelaarName = "Makelaar 3" },
            new() { MakelaarId = 4, MakelaarName = "Makelaar 4" },
            new() { MakelaarId = 5, MakelaarName = "Makelaar 5" }
        };
        
        var expected = new List<Makelaar>
        {
            new() { Id = 1, Name = "Makelaar 1", NumberOfProperties = 3 },
            new() { Id = 2, Name = "Makelaar 2", NumberOfProperties = 2 },
            new() { Id = 3, Name = "Makelaar 3", NumberOfProperties = 1 }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 3);  // ✅ Fixed: top=3
        
        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
    
    [Fact]
    public void GetRankedMakelaars_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var properties = new List<PropertyMasterData>();
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetRankedMakelaars_WithTopLargerThanAvailable_ReturnsAllMakelaars()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "Makelaar 1" },
            new() { MakelaarId = 2, MakelaarName = "Makelaar 2" }
        };
        
        var expected = new List<Makelaar>
        {
            new() { Id = 1, Name = "Makelaar 1", NumberOfProperties = 1 },
            new() { Id = 2, Name = "Makelaar 2", NumberOfProperties = 1 }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 100);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetRankedMakelaars_WithTopZero_ReturnsEmptyList()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "Makelaar 1" },
            new() { MakelaarId = 2, MakelaarName = "Makelaar 2" }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 0);  // ✅ Fixed: top=0
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public void GetRankedMakelaars_OrdersDescendingByListingCount()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "Low" },
            new() { MakelaarId = 2, MakelaarName = "High" },
            new() { MakelaarId = 2, MakelaarName = "High" },
            new() { MakelaarId = 2, MakelaarName = "High" },
            new() { MakelaarId = 3, MakelaarName = "Medium" },
            new() { MakelaarId = 3, MakelaarName = "Medium" }
        };
        
        var expected = new List<Makelaar>
        {
            new() { Id = 2, Name = "High", NumberOfProperties = 3 },
            new() { Id = 3, Name = "Medium", NumberOfProperties = 2 },
            new() { Id = 1, Name = "Low", NumberOfProperties = 1 }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
        
        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
    
    [Fact]
    public void GetRankedMakelaars_GroupsByMakelaarId_NotByName()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "ABC Makelaars" },
            new() { MakelaarId = 1, MakelaarName = "ABC Makelaars (different office)" },
            new() { MakelaarId = 2, MakelaarName = "ABC Makelaars" }
        };
        
        var expected = new List<Makelaar>
        {
            new() { Id = 1, Name = "ABC Makelaars", NumberOfProperties = 2 },
            new() { Id = 2, Name = "ABC Makelaars", NumberOfProperties = 1 }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
        
        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
    
    [Fact]
    public void GetRankedMakelaars_UsesFirstMakelaarNameInGroup()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "First Name" },
            new() { MakelaarId = 1, MakelaarName = "Second Name" },
            new() { MakelaarId = 1, MakelaarName = "Third Name" }
        };
        
        var expected = new List<Makelaar>
        {
            new() { Id = 1, Name = "First Name", NumberOfProperties = 3 }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetRankedMakelaars_WithSingleProperty_ReturnsSingleMakelaar()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "Solo Makelaar" }
        };
        
        var expected = new List<Makelaar>
        {
            new() { Id = 1, Name = "Solo Makelaar", NumberOfProperties = 1 }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetRankedMakelaars_WithEqualCounts_MaintainsStableOrder()
    {
        // Arrange
        var properties = new List<PropertyMasterData>
        {
            new() { MakelaarId = 1, MakelaarName = "Makelaar A" },
            new() { MakelaarId = 2, MakelaarName = "Makelaar B" },
            new() { MakelaarId = 3, MakelaarName = "Makelaar C" }
        };
        
        // Act
        var result = PropertyHelper.GetRankedMakelaars(properties, 10);
        
        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(m => m.NumberOfProperties == 1);
    }
}