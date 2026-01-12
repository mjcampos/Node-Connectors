# Node Connectors

#### A node graph consisting of 5 different node types

## Node Types
<ul>
  <li>Unlocked - Red. This node type is already unlocked and nothing more needs to be done.</li>
  <li>Visible - Blue. Node that can be seen within Hoverable range. They can be clicked and made to Unlocked node. They also carry data that can be viewed when mouse hovers over this node type.</li>
  <li>Non-Hoverable - Green. Node that can be seen within Non-Hoverable range. While they can be seen like Visible nodes player cannot see data when hovering over it. They can automatically convert to Visible node when within range of Hoverable Range.</li>
  <li>Hidden - Grey. Node that exists outside the Non-Hoverable range. This type of node cannot be seen until it is within Hoverable or Non-Hoverable range.</li>
  <li>Locked - Black. When visible this Node type cannot be interacted with. It will not convert to any other node type, even when within range of Hoverable and Non-Hoverable. </li>
</ul>

[Demo Video](https://youtu.be/uyghS28SfnU)
