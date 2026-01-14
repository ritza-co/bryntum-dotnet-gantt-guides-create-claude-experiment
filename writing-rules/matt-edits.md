Style edits specific to Matt - the writer.

## Style Rules

### Repetition

- Vary opening words of consecutive sentences - avoid starting multiple sentences with the same word. For example:

```
- Create a backend ...
- Create a frontend...
```
- Check for repeated phrases/sentences and rephrase duplicates.

### Tense

- Prefer present tense over future tense for conciseness.
- Search for "will" and "'ll" and convert to present tense where possible.

Note: **do not always use present tense**, use your discretion. It’s more important to show the reader the value/usefulness of what we’re doing than to always use present tense. 
For example, do not make a change like this:

```diff
- We'll define database models for the tasks example data using [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/). 
+ The database models for the tasks example data use [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/). 
```

Because by removing the phrase about defining the models (the purpose of the section this example is from), this intro section feels a bit random.

Anther example of what NOT to do:

```diff
- We'll now configure and add a Bryntum Gantt to the frontend starter project.
+ Next, configure and add a Bryntum Gantt to the frontend starter project.
```

This introduction now reads like an instruction, which is confusing because it doesn’t show a task to complete (instead it introduces the instructions in the subsection that this text is from). In these instances, rather use the first person plural used and just change it to present tense (instead of using the imperative form the LLM used here):

```
 Next, we configure and add a Bryntum Gantt to the frontend starter project.
```

### Passive Voice

- Default to active voice verbs.
- Only use passive voice when:
  - The subject is missing or too complicated to explain briefly
  - Active voice would add another clause to an already long sentence
- **Code explanations specifically**: Always use present tense, active voice verbs after code blocks.

### Parallel Structure

- List items must have consistent grammatical structure (all full sentences or all fragments, not mixed).
- Headings at the same level should have the same grammatical structure (e.g., all imperative verbs, like "Create the database" or all gerunds, like "Creating the database").

**Headings using imperative verbs** (giving instructions):
```
## Create the database
## Configure the API
## Run the application
```

**Headings using gerunds** (describing topics/processes):
```
## Creating the database
## Configuring the API
## Running the application
```

**Inconsistent (avoid):**
```
## Create the database
## Configuring the API
## How to run the application
```

**List items using imperative verbs:**
```
- Install the dependencies
- Create a new file
- Run the build command
```

**List items using gerunds:**
```
- Installing the dependencies
- Creating a new file
- Running the build command
```

**Inconsistent (avoid):**
```
- Install the dependencies
- Creating a new file
- How to run the build command
```

Choose one form and use it consistently throughout a list or set of same-level headings.

### Specificity and Conciseness

- Avoid vague phrases like "can be useful" - be specific about the benefit.
- Remove filler words that don't communicate important information (e.g., "handle" when it adds nothing).
- Use "whether" instead of "if" when appropriate for conditionals.

- DO NOT simplify to the point of losing important information. For example, do not make a change like this:

```diff
- The Bryntum Gantt has a [project](https://bryntum.com/products/gantt/docs/api/Gantt/model/ProjectModel)  that handles loading data from and syncing data changes to the .NET backend. This project uses  a specific [sync request structure](https://bryntum.com/products/gantt/docs/guide/Gantt/data/crud_manager#sync-request-structure) for data synchronization.
+ The Bryntum Gantt project loads data from and syncs data changes to the .NET backend using  a specific [sync request structure](https://bryntum.com/products/gantt/docs/guide/Gantt/data/crud_manager#sync-request-structure).
```

The project link was removed, which is important information.

## Consistency Rules

### Code Formatting

- Be consistent with backticks and quotation marks - don't mix styles for similar items.
- Terminal output should be formatted as code.

### Punctuation

- Sentences with the same structure should use the same punctuation pattern.
- Consider moving prepositional phrases to sentence beginnings to avoid comma issues.

### Naming

- Refer to data types (resources, events, etc.) consistently throughout.
- Use correct product name formatting (e.g., "Task Board" not "Taskboard").

### Links and Explanations

- Place links and explanations at the first mention of a concept, not later mentions.