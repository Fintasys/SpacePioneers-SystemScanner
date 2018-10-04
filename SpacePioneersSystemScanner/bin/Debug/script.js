
function init() {
	
	api.load('system');
	setTimeout(function(){ 

		ISystemView.vxUpdateCrossEvent(1,1, null);

		setTimeout(function(){ 

			ISystemView.vxFocus();

		}, 1000);
		
	}, 1000);
	
}


var x = 1;
var y = 1;

function nextSystem() {
	
	if(x < 99)
		ISystemView.vxXRight();
	else {
		
		ISystemView.vxXRight();
		
		setTimeout(function(){ 
			ISystemView.vxYDown();
		}, 1000);
		
	}
}


function parse() { 



}