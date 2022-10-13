# Unity HTTP Debugger
![](Documentation~/EditorWindow.jpg)
Http request logging tool for Unity. 

- Send http requests from Editor Window
  - Save requests and responses
- Log http requests
 
# TODO
## Priorites
- Add/polish features that can't be replacet with extarnal apps like postamn
  - request logging
  - breakpoints
  - fast request/response edit/inspect
## Refactor
1. Finish Exchange inspector
  - ~~nothing selected view~~
  - ~~Wrap request into a scriptable object and use inspector window~~
  - ~~Inspect playmode logs~~
    - ~~read only mode: copy data but not overwrite~~
  - ~~inspect stash requests~~
    - ~~edit and serialize~~
  - ~~send button working~~
  - Add Header with request ID and time?

2. Stash View
  - Design
    - integrate into inspector?

3. Design UX for
  - ~~Inspeting playmode requests~~
    - ~~Wrap request into a scriptable object and use original inspector~~

4. Finish Request Console
  - ~~filter by status and id~~
  - ~~search functionality~~
  - ~~Clear button~~
  - ~~Clear on Play option~~
  - Saving playmode requests to stash
  - Write logs to log file in a readable format, do not serialize HDRequests unless for editor persistant state

5. Design datastore
  - ~~How and where to serialize playmode request logs~~
  - ~~how to access them~~

5. Fix the All-in-one window

## Open Problems
- I can't extract request headers from `UnityWebRequest` object
  - Decorator/wrapper class for `UnityWebRequest`?

## Features
- Editor
    - Serialize data outside EditorPrefs, so that two different project dont share the same data

- Request View
    - Edit Headers
    - Edit Authentication
    - "Format" Button for json body
    - Support different body types

- Response View
    - Raw/Formatted payload toggle
    - XML Response formatter

- Request Console
    - request breakpoints (edit request before it is sent)
    - Add "Clear on Play" toggle
    - Autoscroll down
    - Add stacktrace
        - Double-click to open script
        - Add breakpoints?
    - Add filters
        - Ability to toggle log visibility by ID
        - Categories- Add req IDs to categories and filter logs by them
    - ~~Change list view to Tree view with columns for name, url, response, method, time~~
    - ~~Right click request to "Save to stash"~~     

- Mock responses
    - Window where you can toggle a mock response for requests with a specific IDs

- QoL
    - If reqest url doesn't start with https or http try to add it
    - ~~Fix performance issue when Playmode has bigger dataset~~
    - ~~Remove UITK Aid dependency~~

## References
- [Insomnia](https://github.com/Kong/insomnia)
- [Fiddler](https://imgur.com/SF40wep)
- [console](https://dotnetanalysis.blogspot.com/2012/11/http-status-codes-tutorial.html)