# TODO

## Features
- Runtime API
    - Process Request API
    - Integrate UniTask?

- Editor
    - Serialize data outside EditorPrefs, so that two different project dont share the same data

- Reqest View
    - Edit Headers
    - Edit Authentication

- Response View
    - Raw/Formatted payload toggle
    - XML Response formatter
    - 

- Playmode Tab
    - Change listo to Tree view with columns for name,url,response,method,time (https://dotnetanalysis.blogspot.com/2012/11/http-status-codes-tutorial.html)
    - request breakpoints (edit request before it is sent)
    - Right click request to "Save to stash"
    - Add "Clear on Play" toggle
    - Autoscroll down
    - Add stacktrace
        - Doubleclick to open script
    - Add filters
        - Ability to toggle log visibility by ID
    - Categories
        - Add req IDs to categories and filter logs by them

- Mock Tab
    - request mock values (with enable toggles

- Breakpoint Tab

- QoL
    - If reqest url doesn't start with https or http try to add it
    - Remove UITK Aid dependency
    - Fix performance issue when Playmode has bigger dataset