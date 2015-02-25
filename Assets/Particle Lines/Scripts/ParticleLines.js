//	By Unluck Software	
// 	www.chemicalbliss.com																																			

import System.Collections.Generic;	//Used to sort particle system list

#pragma strict

@script RequireComponent(LineRenderer);

var _lineUpdate:float;									//How often to update line (0=every frame 1=every second)(Use to increase performance if updating every second is not needed)
var _line:LineRenderer;									//Assign this line renderer
var _ps:ParticleSystem;									//Assign Particle System used for line renderer

//Sorting
var _sortParticleOnLife:boolean;						//Sorts line based on lifetime of particles
var _sortParticleOnDistance:boolean = true;				//Sorts line based on distance from center
var _freezeZeroParticle:boolean = true;					//The first particle will freeze zero position, use to avoid jittering in the front of a moving trail
var _centerOddParticles:boolean;						//Centers every other line vertex to create a flowery look

var _gradients:boolean;									//Enable gradient colors		
var _gradientStart : Gradient;							//Gradient color used over time to change the START color of the line
var _gradientEnd : Gradient;							//Gradient color used over time to change the END color of the line
var _gradientSpeed:float = 1;							//How fast colors are cycled
var _randomGradientStart:boolean;						//Starts the gradient at a random position
var _gradientLight:boolean;								//Apply colors to light

var _light:Light;										//Assign Light used in effect
var _vertexCountIntensity:boolean;						//Use the amount of particles to decide how bright the lights are
var _vertexCountIntensityMultiplier:float = 0.01;		//Multiply the intensity
var _flicker:boolean;									//Flicker intensity based on a animation curve
var _lightFlicker:AnimationCurve;						//Flicker animation curve
var _positionLight:String;								//Position lights based on particle positions
															//"random" = finds a random particle 
															//"end"	= finds a particle in the end of the line
														
var _tileLineMaterial:boolean;							//Enable to tile material attached to line renderer
var _tileMultiplier:float = 1;							//Tile material based on vertex length * multplier
var _fixedTileMaterial:boolean;							//Tiling only based on multiplier
var _tileAnimate:boolean;								//Animate tile material
var _tileAnimateSpeed:float;							//Speed of animation

var _scale:boolean;										//Enable to scale line start and end based on curves
var _startScaleMultiplier:float = 1;					//How much to scale the start of the line
var _startScale:AnimationCurve;							//Start animation curve
var _endScaleMultiplier:float = 1;						//How much to scale the end of the line
var _endScale:AnimationCurve;							//End animation curve
var _scaleSpeed:float = 1;								//Speed of animation based on curves

var _rotationSpeed:Vector3;								//Rotate the particle system to create swirls and spirals (Particles must have start speed)

var _sortInterval:int = 2;								//Each frame sorting occurs (1=always, 2=every other frame ...) 

private var _lineCounter:float;							//Used to control _lineUpdate
private var _lineVertex:int;							//Saves how many verts the line uses																																																																																																																																																																																																																																																																																																																																																																															
private var myParticles : ParticleSystem.Particle[];	//Populated with information about each particle
private var _gradientCounter:float;						//Time counter for cycling colors
private var _randomNumber:float;						//Random value generated at start (Used to avoid uniformed scaling and light flicker on lines that are instantiated on the same frame)
private var _saveLightIntensity:float;					//Saves the initial light intensity at start

var _saveMat:Material;
private var _randomInt:int;

var _cutEndSegments:int=4;								//Reduces lenght of line to avoid looping back to start segment

function Start () {
	if(!_line)	_line 	= transform.GetComponent(LineRenderer);
	if(!_ps)	_ps 	= transform.parent.GetComponent(ParticleSystem);
	if(_light)
	_saveLightIntensity = _light.intensity;
	_randomNumber = Random.value;
	_randomInt = _randomNumber*10;
	_lightFlicker.postWrapMode= WrapMode.Loop;
	_startScale.postWrapMode= WrapMode.Loop;
	_endScale.postWrapMode= WrapMode.Loop;
	if(_randomGradientStart)
	_gradientCounter = _randomNumber;
}

function GetFrameCount():int {
	return Time.frameCount + _randomInt;	//Randomize framecount to avoid all instances of ParticleLines to sort on the same frame. (reduce performance spikes)
}

function Compare (first : float, second : float){
	return second.CompareTo (first);
}

function Clamp(value:int, min:int){
    if (value < min){
    	value = min;
    }
    return value;
}

function SetLine(){
	for (var j:int; j < _lineVertex -_cutEndSegments; j++){	
		_line.SetPosition(j, myParticles[j].position);
	}
}

function SetLineFlower(){
	for (var i:int; i < _lineVertex-_cutEndSegments; i++){	
		if(i%2==0)
			_line.SetPosition(i, myParticles[i].position);
		else
			_line.SetPosition(i, this._ps.transform.position);
	}
}

function SortLifetime(){
	myParticles.Sort(myParticles,function(g1,g2) Compare(g1.lifetime, g2.lifetime));
}

function SortDistance(){
	myParticles.Sort(myParticles,function(g1,g2) Compare(Vector3.Distance(transform.position, g2.position),Vector3.Distance(transform.position, g1.position)));
}

function LinePos () {
	_lineVertex = _ps.particleCount;
	myParticles = new ParticleSystem.Particle[_lineVertex];
	_ps.GetParticles (myParticles);
	_line.SetVertexCount(Clamp(_lineVertex-_cutEndSegments,0));
	
	if(_sortInterval > 1 && _sortParticleOnLife && _lineVertex>2){
		if(GetFrameCount() % _sortInterval ==0){
			SortLifetime();
			_ps.SetParticles(myParticles, myParticles.Length);	
		}
	}else if(_sortInterval > 1 && _sortParticleOnDistance && _lineVertex>2){
		if(GetFrameCount() % _sortInterval ==0){
			SortDistance();
			_ps.SetParticles(myParticles, myParticles.Length);		
		}
	}else if(_sortParticleOnLife){	
			SortLifetime();
	}else if(_sortParticleOnDistance){
			SortDistance();
	}
	
	if(_freezeZeroParticle && _lineVertex>0)
		myParticles[0].position = this._ps.transform.position;
	
	if(!_centerOddParticles){
		SetLine();
	}else{
		SetLineFlower();
	}
}

function Update () {
	_lineCounter += Time.deltaTime;
	if(_lineCounter > _lineUpdate){
		LinePos ();
		_lineCounter = 0;
	}
	
	if(_gradients){	
		if(_gradientCounter < 1)		
			_gradientCounter += Time.deltaTime*_gradientSpeed;
		else
			_gradientCounter =0;
		_line.SetColors(_gradientStart.Evaluate(_gradientCounter),_gradientEnd.Evaluate(_gradientCounter) );
		if(_light && _gradientLight){
			_light.color = _gradientStart.Evaluate(_gradientCounter);		
		}
	}
	
	if(_tileLineMaterial && _fixedTileMaterial){
			_line.renderer.material.mainTextureScale.x = _tileMultiplier;
		}else if(_tileLineMaterial){
			_line.renderer.material.mainTextureScale.x = (_lineVertex-_cutEndSegments)*_tileMultiplier;
		}
			
	if(_tileAnimate){
			_line.renderer.material.mainTextureOffset.x = (_line.renderer.material.mainTextureOffset.x + _tileAnimateSpeed*Time.deltaTime)%1;
	}
	
	if(_light){
		if(_positionLight == "end" &&_ps.particleCount>5){
			_light.transform.position = Vector3.Lerp(_light.transform.position, myParticles[_ps.particleCount-6].position, Time.deltaTime*10);	
		}else if(_positionLight == "random" && _ps.particleCount>5){
			_light.transform.position = Vector3.Lerp(_light.transform.position, myParticles[Random.Range(0, _ps.particleCount)].position, Time.deltaTime*2);
		}	
		if(_flicker){
			_light.intensity = _lightFlicker.Evaluate(Time.time+_randomNumber)*_saveLightIntensity;
		}
	}
	
	if(_scale){						
		var t:float = (Time.time*_scaleSpeed)+_randomNumber;
		_line.SetWidth(_startScale.Evaluate(t)*_startScaleMultiplier, _endScale.Evaluate(t)*_endScaleMultiplier);
	}	
	
	if(_rotationSpeed.magnitude > 0) 	
		_ps.transform.Rotate(_rotationSpeed*Time.deltaTime);
	
	if(this._vertexCountIntensity)		
		_light.intensity = _saveLightIntensity*this._vertexCountIntensityMultiplier*this._lineVertex;	
}

//Test

//var _start:GameObject;
//var _end:GameObject;
//var sortParticle:ParticleSystem.Particle[];

//	GetLastParticles();
		//	sortParticle.Sort(sortParticle,function(g1,g2) Compare(g1.lifetime, g2.lifetime));
		//	SetLastParticles();
		//	_ps.SetParticles(myParticles, myParticles.Length);

//function TargetLine () {
//	var distance:float = Vector3.Distance(_start.transform.position,_end.transform.position);
//	var c:float = 1f/_lineVertex;
//	for(var i :int = 0; i < _lineVertex; i++){
//		var pos : float = distance*c*i;
//		myParticles[i].position.z = pos;
//	}
//	_ps.emissionRate = Mathf.FloorToInt(distance);
//	_ps.transform.position = _start.transform.position;
//	_ps.transform.LookAt(_end.transform,transform.up);
//	_ps.SetParticles(myParticles, myParticles.Length);
//	
//}

//Moved to Editor Script

//function OnDrawGizmosSelected () {	
//	if(!Application.isPlaying){
//		if(!_line)	_line = transform.GetComponent(LineRenderer);
//		_line.SetVertexCount(0);
//		if(_line.renderer.sharedMaterial != null && _saveMat != _line.renderer.sharedMaterial)
// 		_saveMat = _line.renderer.sharedMaterial;
// 		if(_line.renderer.sharedMaterial == null)
// 		_line.renderer.sharedMaterial = _saveMat;
//	}
//}

//function GetLastParticles(){
//	var p:int = 50;
//	sortParticle = new ParticleSystem.Particle[p];
//	if(myParticles.Length > p){
//	for(var i:int; i < p; i++){		
//		sortParticle[i] = myParticles[myParticles.Length -i -1];
//	}
//	}
//}
//
//function SetLastParticles(){
//	var p:int = 50;
//	if(myParticles.Length > p){
//	for(var i:int; i < p; i++){
//		myParticles[myParticles.Length -i-1] = sortParticle[i];
//	}
//	}
//}