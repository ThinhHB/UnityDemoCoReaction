# Coreaction - another solution for "multi-state" in game

I've been working with some Platfomer2D games, and I've used StateMachine for Character control. At the begining of the project, I thought the StateMachine will just simple like this :

![Alt Text](https://cloud.githubusercontent.com/assets/9117538/20307846/f03988f0-ab73-11e6-8914-3394546f0c0b.png)

But then, when project grew up, I got some mess like this :

![Alt_Text](https://cloud.githubusercontent.com/assets/9117538/20307919/4384788a-ab74-11e6-8154-1994552bcd01.jpg)

# The weakness of StateMachine

I see that, StateMachine has some weak points

**"Single thread"**

There's only one State can be run at a time, and we have to do many things in that state, and also checking to transition to other states

**Hard to extend**

When I want to add another state, I have to modify the other states too, because they're strong coupling together.


# Learn from Unity's Animator component

Look at Unity's Animator component, I see there're two definition : **The parameters** and **Transition conditions**
We define some parameter, we declare the transition between States, then, we just modify the value of Parameters (via *SetTrigger(), SetFloat(), SetBool()* .v.v..) and the Animator will select the right State to run, base on conditions.


# The idea about Co-reactions

Here's a very simple Platformer2D, the character just have moving and jump action.

![Alt_text](https://cloud.githubusercontent.com/assets/9117538/20308451/90d614de-ab76-11e6-8c39-b545014869ca.png)

There're 2 reaction : Moving and Jump

**Moving**

+ Action : add horizon force to make character moving

+ Condition :

    Player hold A or D key (Input.GetKey(A || D) = true)


**Jump**

+ Action : add vertical force to make character jumping

+ Condition :

    Player press W(Input.GetKeyDown(A || D) = true)

    Player are on ground (OnGround = true)


# From idea to script

And this's is how I declare the reaction for character

**Define Parameters**

```java
ReactionController reactionCtrl = gameObject.AddComponent<ReactionController>();

reactionCtrl.AddParameter(ParameterID.OnGroundState, ParameterType.Boolean, false);
reactionCtrl.AddParameter(ParameterID.HoldLeftKey, ParameterType.Boolean, false);
reactionCtrl.AddParameter(ParameterID.HoldRightKey, ParameterType.Boolean, false);
reactionCtrl.AddParameter(ParameterID.PressJumpKey, ParameterType.Trigger);
```

**Write the basic Action that will be used in Reaction**

```java
void Jump()
{
	var velo = controlRigidbody.velocity;
	velo.y = jumpSpeed;
	controlRigidbody.velocity = velo;
}

void Move(float XaxisRaw)
{
	var velocity = controlRigidbody.velocity;
	velocity.x += movingAccelerate * XaxisRaw * Time.deltaTime;
	controlRigidbody.velocity = velocity;
}

void RevertRender(bool isToLeft)
{
	var localScale = transform.localScale;
	var ratio = isToLeft ? -1f : 1f;
	localScale.x = Mathf.Abs(localScale.x) * ratio;
	transform.localScale = localScale;
}
```

**Define Reaction, create conditions from parameters**

```java
void AddJumReaction()
{
	var jump = new BaseReaction();
	jump.AddCondition(ParameterID.PressJumpKey, CompareType.Trigger, null);
	jump.AddCondition(ParameterID.OnGroundState, CompareType.BooleanTrue);
	jump.SetAction(Jump);
	_reactionCtrl.AddReaction(jump);
}

void AddMoveLeftReaction()
{
	var moveLeft = new BaseReaction();
	moveLeft.AddCondition(ParameterID.HoldLeftKey, CompareType.BooleanTrue);
	moveLeft.AddCondition(ParameterID.VeloX, CompareType.FloatGreater, -movingLimitSpeedX);
	moveLeft.SetAction(() => Move(-1f));
	_reactionCtrl.AddReaction(moveLeft);
}

void AddMoveRightReaction()
{
	var moveRight = new BaseReaction();
	moveRight.AddCondition(ParameterID.HoldRightKey, CompareType.BooleanTrue);
	moveRight.AddCondition(ParameterID.VeloX, CompareType.FloatLess, movingLimitSpeedX);
	moveRight.SetAction(() => Move(1f));
	_reactionCtrl.AddReaction(moveRight);
}
```

From then, It'll easier for me when I want to add more reactions, or more condions or parameters. My demo is very simple, here I just paste some main part of my code.
