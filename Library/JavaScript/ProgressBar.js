
ProgressBar = function(parent, width, fontSize) {
  // constructor for Progress Bar object; 'width' and 'fontSize' in pixels
  this.pixels = width;
  this.outerDIV = document.createElement("div");
  this.outerDIV.className = "progressBarContainer";
  this.outerDIV.style.width = width + "px";
  parent.appendChild(this.outerDIV);
  
  this.fillDIV = document.createElement("div");
  this.fillDIV.className = "progressBarFill";
  this.outerDIV.appendChild(this.fillDIV);
  
  this.progressIndicator = document.createElement("div");
  this.progressIndicator.className = "progressBarText";
  this.progressIndicator.style.width = width + "px";
  this.outerDIV.appendChild(this.progressIndicator);
  }
  
ProgressBar.prototype.setPercent = function(pct) {
  // expects 'pct' values between 0.0 and 1.0
  var fillPixels;
  if (pct < 1.0) fillPixels = Math.round(this.pixels * pct);
  else { // avoid round off error
    pct = 1.0;
    fillPixels = this.pixels;
    }
  this.progressIndicator.innerHTML = Math.round(100 * pct) + "%";
  this.fillDIV.style.width = fillPixels + "px";
  }
