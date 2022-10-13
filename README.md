# Unity HTTP Debugger
![](Documentation~/EditorWindow.jpg)
HTTP request logging tool for Unity. 

> Supported in Unity 2022+ (may be backported at the later stages of the project)
## Features

### Runtime Extensions
- Extension methods to Log web requests
- Logged requests are visible in the *Request Console Window*

### Request Console
> Open from context menu: _Http Debugger/Console_
- Log, filter, search, and preview web requests from your game/app
- Save chosen requests and responses to *Request Stash*

### Request Inspector
> Open by selecting request in Console or Stash windows
- Preview and edit your request/response
- Send requests and save responses 

### Request Stash
> Open from context menu: _Http Debugger/Stash_
- Go back to your saved requests to run and or edit them
 
## TODO

### Priorities
- Add/polish features that can't be replaced with external apps like Postman
  - request logging
  - breakpoints
  - fast request/response edit/inspect

### Features
> listed in priority order

#### Request Console
- Write logs to file in a readable format, do not serialize HDRequests unless for editor persistent state
- request breakpoints (edit request before it is sent)
- Add stack trace
  - Double-click to open script
- Categories- Add req IDs to categories and filter logs by them

#### Misc
- Mock responses
    - Window where you can toggle a mock response for requests with specific IDs

#### Exchange inspector
- Add Header with request ID and time?
- "Format" Button for body
- Support more than JSON body
- CodeField
  - Better scroll view

- Request View
  - Edit Headers
  - Edit Authentication

- Response View
  - Raw/Formatted payload toggle
  - XML Response formatter
- If the request URL doesn't start with HTTPS or HTTP try to add it (very low priority)

#### Stash View
> Stash view is less important right now as Postman does it 100 times better.
> Development should focus on features unique to this tool.

#### All-in-One window
- create an editor window that contains all views like it was at the beginning of the project

## Open Problems
- I can't extract request headers from the `UnityWebRequest` object
  - Decorator/wrapper class for `UnityWebRequest`?

## References
- [Insomnia](https://github.com/Kong/insomnia)
- [Fiddler](https://imgur.com/SF40wep)
- [console](https://dotnetanalysis.blogspot.com/2012/11/http-status-codes-tutorial.html)