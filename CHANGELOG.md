## Changes

### Created Bryntum Gantt guides (Vanilla TypeScript and React)

Prompt:
Create through Claude.md and implement the instructions.

Summary:
1. **Created `/draft.md`** - Vanilla TypeScript Bryntum Gantt with .NET and SQLite guide
   - Adapted from the Calendar guide to use Gantt-specific code
   - Updated all models: `GanttTask` model with parent-child relationships, `SyncModels.cs` with `Optional<T>` for partial updates
   - Created `GanttContext.cs` with cascade delete for hierarchical tasks
   - Created `GanttController.cs` with load/sync endpoints
   - Updated frontend config to use `@bryntum/gantt`, `transformFlatData`, and Gantt columns
   - Changed port from 5000 to 1337 to match working code

2. **Created `/draft-react.md`** - React Bryntum Gantt with .NET and SQLite guide
   - Same backend as vanilla guide
   - Updated frontend to use `@bryntum/gantt-react` and `BryntumGanttProps`
   - Updated `App.tsx` to render `BryntumGantt` component

3. **QA Validation** - Applied guide changes to QA folders:
   - `/QA/dotnet-sqlite-gantt-starter/` - Backend files (Models, Data, Controllers, Program.cs, appsettings.json)
   - `/QA/gantt-vanilla-starter/` - Frontend files (ganttConfig.ts, main.ts, style.css)
   - `/QA/gantt-react-starter/` - Frontend files (ganttConfig.ts, App.tsx, style.css)
   - Verified all files match the working code in `/code/` directories

### Proofread guides against writing rules

Prompt:
Ok great. Now proof read @draft.md and @draft-react.md using the writing rules in /writing-rules

Summary:
Applied writing rules from `AI-writing-style-kill-list.md` and `ritza-writing-rules.md`:
1. Changed "You'll learn to do the following" → "We'll cover the following" (use first person plural "we" voice)
2. Changed "simplifies loading data" → "handles loading data" (avoid words close to "simply" on kill list)
3. Changed vague "which can be useful" → "for programmatic control" (be specific, avoid fluff)