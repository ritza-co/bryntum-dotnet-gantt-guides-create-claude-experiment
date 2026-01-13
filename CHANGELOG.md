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