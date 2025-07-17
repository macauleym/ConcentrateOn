# ConcentrateOn

## Name

concentrate - a simplistic generalized tool to help focus on the subjects a user wants to study/work on/look into/etc on different days during the week.

## Synopsis

`concentrate [command] [argument] [options]`

## Description

The overall goal of this project is to be a very light-weight application to track the topics/subjects that you want concetration for.

This is not meant to be a to-do application, the goal is a much higher abstraction. It's not meant to track time (though you can mark a desired time you want to focus on something), nor to mark things as "done". It is only for keeping track of the things you want to keep focus on for each day.

## Commands

### On

This is the core command to add new subjects, and associate them with the desired days of the week.

#### Arguments

Subject

&nbsp;&nbsp;&nbsp;&nbsp;The string name of the subject the user wishes to focus on during the week.

#### Options

-p/--priority

&nbsp;&nbsp;&nbsp;&nbsp;An integer value to help sort a Subject, and to list which ones the user wishes to focus on above others.

&nbsp;&nbsp;&nbsp;&nbsp;Valid values are the individual days of the week, as well as the relativistic values 'yesterday', 'today', and 'tomorrow'.

--days [required, unless passing -r/--forget]

&nbsp;&nbsp;&nbsp;&nbsp;The comma (',') delimited list of days of the week to associate the given subject to.

-d/--during

&nbsp;&nbsp;&nbsp;&nbsp;The relative time of day you want to try and focus on the subject.

&nbsp;&nbsp;&nbsp;&nbsp;Valid values are 'morning', 'afternoon', and 'night'.

-t/--duration

&nbsp;&nbsp;&nbsp;&nbsp;A string representing how long you want to spend on the subject.

&nbsp;&nbsp;&nbsp;&nbsp;As this is not attempted to be parsed into an actual `DateTime` object, this can truly be whatever the user wants.

&nbsp;&nbsp;&nbsp;&nbsp;Some valid options could be: "15m", "2h", "3 hours", "45 seconds", "a decent amount of time", "at least a couple minutes, but no more than like 12", etc.

-r/--forget [required, unless passing --days]

&nbsp;&nbsp;&nbsp;&nbsp;Tells the application to remove the given subject from all associated days 

&nbsp;&nbsp;&nbsp;&nbsp;If it exists, and is associated with any days.


### for

#### Arguments

Day

&nbsp;&nbsp;&nbsp;&nbsp;The given day to display all the associated subject for.

## Examples

To add a new subject for a desired day.

```bash
concentrate on drawing \
  --days Monday,Wednesday,Friday \
  --during afternoon \
  --duration 15m \
  --priority 4
```

To update an existing subject with different  properties.

```bash
concentrate on drawing \
  --days Monday,Friday,Saturday \
  --duration 30m \
  --priority 3
```

To remove a subject association from all days.

```bash
concentrate on building --forget
```

To grab the list of subjects for a given day.

```bash
concentrate for Thursday
```

```bash
concentrate for tomorrow
```

## Structure

### /concentrate

This is the CLI application that is executed by the user.

### /ConcentrateOn.Core

Contains all of the logical pieces needed to support the application.

Anything from models, to interfaces, to data gathering/saving, etc.

### /ConcentrateOn.Test

Self-explanatory. Contains all of the unit tests for the logical bits of the application.
