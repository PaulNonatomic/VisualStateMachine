# Change Log

## [0.8.6-beta] - Jan 17, 2023
- Hit fix for build failure

## [] - Dec 23, 2023
- Added State Machine Editor redraw on assembly reload

## [0.8.5-beta] - Dec 21, 2023
- Merged in PR from IGB to fix errors being suppressed by not awaiting an async method

## [0.8.4-beta] - Dec 21, 2023
- Added a scroll panel to the StateNodeView so nodes don't get too big when the property list expands. Also solves the issue of the node boundary growing bizarly large when a list is added to the property container

## [0.8.3-beta] - Dec 18, 2023
- Merged in PR from IGB for Added amends to stabilise the State Machine Editor and reduce the risk of accidental deletions and errors

## [0.8.2-beta] - Dec 17, 2023
- Fix for null StateMachine when attempting to fix a null statemachine. Stupid!
- Removed the validation of connections from the StateMachine as it was removing valid connections. Will circle back to this.

## [0.8.1-beta] - Dec 14, 2023
- Fix for null StateMachine in editor when exiting run time from a sub state machine

## [0.8.0-beta] - Dec 14, 2023
- Breaking change renamed InitializeState to AwakeState and added StartState

## [0.7.14-beta] - Dec 14, 2023
- Prevented state initialization from being called if application not running

## [0.7.13-beta] - Dec 14, 2023
- Added a DestroyState method to States to allow for cleanup when the StateMachine is destroyed

## [0.7.12-beta] - Dec 13, 2023
- Added PortData clone method

## [0.7.11-beta] - Dec 13, 2023
- Made frame delay on transition optional and configurable via the Transition attribute

## [0.7.10-beta] - Dec 13, 2023
- Added additional checks for running in playmode in the statemachine

## [0.7.9-beta] - Dec 13, 2023
- hot fix to ensure nodes exit before the frame delay is applied

## [0.7.8-beta] - Dec 13, 2023
- added a glow to nodes that fades out as they exit
- introduced port colours which influence edge colours
  - By default ports are coloured to match their nodes 
    unless a colour is passed to the transition attribute
- Discovered that Jumps can cause stack over flow exceptions so introduced a
    frame delay between transitions to allow the stack to clear

## [0.7.7-beta] - Dec 11, 2023
- Updated the JumpNode to update the valid JumpId's on mouse over and mouse down
 
## [0.7.6-beta] - Dec 11, 2023
- Added NodeWidth nodes to the sub state machine states

## [0.7.5-beta] - Dec 11, 2023
- Added support for parallel sub state machines

## [0.7.4-beta] - Dec 10, 2023
- Added a fix for handling transition name changes

## [0.7.3-beta] - Dec 10, 2023
- Improved StateSelectWindow search by comparing lower case search query again lowercase state names
- State names are now processed to be more readable in the StateSelectorWindow
- Added support for keyboard controls in the StateSelectorWindow

## [0.7.2-beta] - Dec 10, 2023
- Added better support for removal of nodes and connections when states are missing/deleted

## [0.7.1-beta] - Dec 10, 2023
- Remove the need for SubStateMachines to flag reinitialization
  - All nodes are only initialized once
- Added new node icons[StateSelectorWindow.uss](Source%2FVisualStateMachine%2FEditor%2FResources%2FStateSelectorWindow.uss)
- Hidden relay nodes from the selection menu. Will eventually deprecate them.

## [0.7.0-beta] - Dec 10, 2023
- Reduced the state node max character li[StateSelectorWindow.uss](Source%2FVisualStateMachine%2FEditor%2FResources%2FStateSelectorWindow.uss)mit to 30
- Introduced jump nodes to simplify loops and other complex transitions

## [0.6.5-beta] - Dec 08, 2023
- Added additional save points when connections are created

## [0.6.4-beta] - Dec 07, 2023
- Applied max node width to width as well
- 
## [0.6.3-beta] - Dec 07, 2023
- Fix for the NodeWidth attribute being applied to the property container and not the node as a whole

## [0.6.2-beta] - Dec 07, 2023
- Prevented BaseSubStateMachine selecting the StateMachine if already selected

## [0.6.1-beta] - Dec 07, 2023
- BaseSubStateMachine now selects the sub state machine on update if it is not already selected

## [0.6.0-beta] - Dec 06, 2023
- Refactor of SubStateMachine with new base class BaseSubStateMachine
  - Allows custom transition ports to be added to derived versions of BaeSubStateMachine
- Refactor of StateMachineController to abstract StateMachineCore
  - Removes dependency upon MonoBehaviour and empowers BaseSubStateMachine to be used in non MonoBehaviour contexts
- Added an InitializeState method to States to allow for setup ahead of the call to EnterState
- Converted the Update method of States to virtual removing it's requirement
- Fix for deep nested sub state machines allowing the StateMachineEditor to be opened automatically 
- Added support for the EndState of a StateMachine to be passed to it's parent BaseSubStateMachine

## [0.5.9-beta] - Dec 05, 2023
- Fix for relay node sizes

## [0.5.8-beta] - Dec 05, 2023
- Set max node width to 300px
- Added title filtering to StateNodes

## [0.5.7-beta] - Dec 05, 2023
- Abstracted a BaseSubStateMachine to better support custom sub state machine nodes that may want to customise their transitions
- Fix for nodes with properties and long names not filling the property container

## [0.5.6-beta] - Dec 05, 2023
- Fix for the state selector window displaying empty groups when searching. 
- Fix for groups found in searches being folded.
- Fix for property container width not spanning full node width when there are no properties

## [0.5.5-beta] - Nov 29, 2023
-Exposed the current state in the StateMachine and in the StateMachineController

## [0.5.4-beta] - Nov 29, 2023
- fix issue with progress bar not being displayed
- refactored the StateMachineGraphView to better separate concerns

## [0.5.3-beta] - Nov 27, 2023
- Seperated the graph orientation from port orientation to better separate editor scripts from runtime.

## [0.5.2-beta] - Nov 27, 2023
- Fix for introduced build errors

## [0.5.1-beta] - Nov 27, 2023
- Hid the samples directory (which still needs actual samples)
- 
## [0.5.0-beta] - Nov 26, 2023
- Support for vertical and flipped directional relay nodes
- The state selection menu now appears where the mouse was when it was opened
- Fix for input and output ports being able to connect to their own kind
- Fix for the create state button becoming broken when docking/undocking the state machine editor
- Added a fix for edge positions when dragging out
- Fix for inverted edges. This was a real challenge to understand.
- Fix for bug where connection data could be lost when switching state machines
- Fixed the adding and positioning of new edges by moving the creation process into the StateEdgeListener. 
  This required the graph views position to be used to offset the new edge.
- Support for custom state nodes
- Added revised node colours
- Added support for inherited attributes

## [0.4.3-beta] - Nov 24, 2023
- Hotfix for hidden state icons
- 
## [0.4.2-beta] - Nov 24, 2023
- The StateSelectorWindow now focuses on the ToolbarSearchField for instant searching
- Added a more dedicated service for icons and moved some icon directories around
- Added an editor tool to toggle the samples directory tilde prefix to make my life easier

## [0.4.1-beta] - Nov 23, 2023
- Filtered out abstract states from the state selector 

## [0.4.0-beta] - Nov 23, 2023
- Added new state icons. Built in states are displayed at the top.
- Added focus loss to close StateSelectorWindow. 
- Added new state selector styling. 
- Added state selector state grouping.

## [0.3.6-alpha] - Nov 23, 2023
- Graph view is now drawn on editor update even in edit mode
    - This has the advantage of removing the progress state when exiting runtime

## [0.3.5-alpha] - Nov 23, 2023
- Critical hotfix for the graphview not updating

## [0.3.4-alpha] - Nov 23, 2023
- Fixed persistence of graph view position and scale

## [0.3.3-alpha] - Nov 22, 2023
- Fix for nodes not being drawn when the graph view is first opened

## [0.3.2-alpha] - Nov 22, 2023
- Hotfix for broken edge persistence
- 
## [0.3.1-alpha] - Nov 22, 2023
- Removed some test code for manipulating edge rendering

## [0.3.0-alpha] - Nov 21, 2023
- Improved relay nodes
- Fix for scale and position of graph view for new statemachines
- Fix for blank port labels
- Removed entry port from entry node
- Enforced use of an entry node
- Removed option to set entry node

## [0.2.1-alpha] - Nov 21, 2023
- Fix for SubStateMachine not cleaning up when exiting

## [0.2.0-alpha] - Nov 21, 2023
- Added support for inherited properties in States
- Added a delay and end node
- Fix a bug in the StateMachineController when sub state machines complete

## [0.1.4-alpha] - Nov 21, 2023
- Changed the directory structure and names to avoid Editor namespace conflic

## [0.1.3-alpha] - Nov 21, 2023
- Updated the package.json unity version
- Added support for creating nodes when dropping a ghost edge

## [0.1.2-alpha] - Nov 20, 2023
-  Added support for switching the displayed statemachine when entering and exiting a sub state machine
-  Converted node titles from pascal to title case
-  Added support for custom node colours and label text

## [0.1.1-alpha] - Nov 20, 2023
-  Added changelog, readme and package.json

## [0.1.0-alpha] - Nov 20, 2023
-  Fix for attempting to load graph view state from a null statemachine

