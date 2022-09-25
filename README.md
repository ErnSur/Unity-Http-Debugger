# Unity HTTP Debugger
![](Documentation~/EditorWindow.jpg)
Http request logging tool for Unity. 

- Send http requests from Editor Window
  - Save requests and responses
- Log http requests
 
# TODO

## Refactor
> Refactor to split the window into two/three standalone windows
> 1. Request Console (console for request logs from the game)
> 2. Exchange Inspector (Preview or edit a request-response pair)
> 3. Stash View (save and test reqests for debugging)
> 
> To ease the development I'm updating the project to unity 2022. Mainly because of MultiColumnListView, I could use IMGUI TreeView instead but it would be a huuuge pain.

1. Finish Exchange inspector (wip)
  - send button working
  - read only mode
  
2. Design UX for
  - Inspeting playmode requests
  - Saving playmode requests to stash
  - Stash View
    - integrated into inspector?

3. Finish Request Console
  - Clear button
  - search functionality
  - Clear on Play option

4. Design datastore
  - How and where to serialize playmode request logs
  - how to access them

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
    - Change list view to Tree view with columns for name, url, response, method, time (wip)
    - ~~Right click request to "Save to stash"~~     

- Mock responses
    - Window where you can toggle a mock response for requests with a specific IDs

- QoL
    - If reqest url doesn't start with https or http try to add it
    - Fix performance issue when Playmode has bigger dataset (wip)
    - ~~Remove UITK Aid dependency~~

## References
- [Insomnia](https://github.com/Kong/insomnia)
- [Fiddler](https://imgur.com/SF40wep)
- [console](https://dotnetanalysis.blogspot.com/2012/11/http-status-codes-tutorial.html)