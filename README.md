# TODO
- Update UI-Toolkit-plus to trim double "-"
`       Regex.Replace(input, "-+.", m => char.ToUpper(m.Value[m.Length-1]).ToString());`
- Fix playmode clear button
    - Add unbind method to Exchange,reqest and response views
## Features
- Runtime API
    - Process Request API
    - Integrate UniTask?

- Sidebar
    - Search function
    - Add response code to reqbuttonsmall

- Reqest View
    - Edit Headers
    - Edit Authentication

- Response View
    - Raw/Formatted payload toggle

- Playmode Tab
    - request breakpoints (edit request before it is sent)
    - Right click request to "Save to stash"
    - Add "Clear on Play" toggle

- Mock Tab
    - request mock values (with enable toggles

- Breakpoint Tab

- QoL
    - If reqest url doesn't start with https or http try to add it
    - Remove UITK Aid dependency