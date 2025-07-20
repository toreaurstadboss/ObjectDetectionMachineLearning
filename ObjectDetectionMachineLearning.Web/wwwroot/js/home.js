var colorPalette = [
    "red", "yellow", "blue", "green", "fuchsia",
    "moccasin", "purple", "magenta", "aliceblue",
    "lightyellow", "lightgreen"
];


function rescaleCanvas() {
    var img = document.getElementById('PreviewImage');
    var canvas = document.getElementById('PreviewImageBbox');

    // Get the displayed size (not natural/original size)
    var displayWidth = img.clientWidth;
    var displayHeight = img.clientHeight;

    // Set canvas size to match displayed image size
    canvas.width = displayWidth;
    canvas.height = displayHeight;
}

function LoadBoundingBoxes(objectDescriptions) {
    if (!objectDescriptions) {
        alert('No objects found in image.');
        return;
    }

    //debugger

    console.log(new Date() + ' ' + 'home.js : Loading bounding boxes from returned results ..')

    var objectDesc = typeof objectDescriptions === "string"
        ? JSON.parse(objectDescriptions)
        : objectDescriptions;

    var canvas = document.getElementById('PreviewImageBbox');
    var img = document.getElementById('PreviewImage');
    var ctx = canvas.getContext('2d');

    // Get scaling factors
    var scaleX = canvas.width / img.naturalWidth;
    var scaleY = canvas.height / img.naturalHeight;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

    ctx.font = "10px Verdana";

    console.log(`ctx.drawImage Canvas width: ${canvas.width} Canvas height: ${canvas.height} ScaleX ${scaleX} ScaleY ${scaleY}`);

    for (var i = 0; i < objectDesc.length; i++) {
        const obj = objectDesc[i];

        const x = obj.X * scaleX;
        const y = obj.Y * scaleY;
        const width = obj.Width * scaleX;
        const height = obj.Height * scaleY;

        ctx.beginPath();
        ctx.strokeStyle = "black";
        ctx.lineWidth = 1;
        ctx.fillText(obj.Name, x + width / 2, y + height / 2);
        ctx.fillText("Confidence: " + obj.Confidence, x + width / 2, 10 + y + height / 2);
    }

    for (var i = 0; i < objectDesc.length; i++) {
        const obj = objectDesc[i];

        const x = obj.X * scaleX;
        const y = obj.Y * scaleY;
        const width = obj.Width * scaleX;
        const height = obj.Height * scaleY;

        ctx.fillStyle = getColor();
        ctx.globalAlpha = 0.2;
        ctx.fillRect(x, y, width, height);

        ctx.globalAlpha = 1.0;
        ctx.lineWidth = 3;
        ctx.strokeStyle = "blue";
        ctx.strokeRect(x, y, width, height);

        ctx.fillStyle = "black";
        ctx.fillText("Color: " + getColor(), x + width / 2, 20 + y + height / 2);
    }

    console.log('Bounding boxes:', objectDesc);
    // ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

}

function getColor() {
    var colorIndex = Math.floor(Math.random() * colorPalette.length);
    return colorPalette[colorIndex];
}


function InitLoadBoundingBoxes(objectDescriptions) {
    const img = document.getElementById('PreviewImage');

    const draw = () => {
        setTimeout(() => {
            LoadBoundingBoxes(objectDescriptions);
        }, 1000); // 100ms delay
    };

    if (!img.complete) {
        img.onload = draw;
    } else {
        draw();
    }
}

function LoadBoundingBoxes(objectDescriptions) {
    if (!objectDescriptions) {
        alert('No objects found in image.');
        return;
    }

    // debugger

    var objectDesc = typeof objectDescriptions === "string"
        ? JSON.parse(objectDescriptions)
        : objectDescriptions;

    rescaleCanvas();

    var canvas = document.getElementById('PreviewImageBbox');
    var img = document.getElementById('PreviewImage');
    var ctx = canvas.getContext('2d');

    // Draw the image first
    ctx.drawImage(img, 0, 0);

    ctx.font = "10px Verdana";
    ctx.shadowColor = "black";
    ctx.shadowBlur = 5;

    for (var i = 0; i < objectDesc.length; i++) {
        const obj = objectDesc[i];

        // Draw label and confidence
        ctx.beginPath();
        ctx.strokeStyle = "white";

        ctx.fillStyle = "white";

        ctx.lineWidth = 1;
        ctx.fillText(obj.Name, obj.X + obj.Width / 2, obj.Y + obj.Height / 2);
        ctx.fillText("Confidence: " + obj.Confidence, obj.X + obj.Width / 2, 10 + obj.Y + obj.Height / 2);
    }

    for (var i = 0; i < objectDesc.length; i++) {
        const obj = objectDesc[i];

        ctx.fillStyle = getColor();
        ctx.globalAlpha = 0.2;
        ctx.fillRect(obj.X, obj.Y, obj.Width, obj.Height);

        ctx.globalAlpha = 1.0;
        ctx.lineWidth = 3;
        ctx.strokeStyle = "blue";
        ctx.strokeRect(obj.X, obj.Y, obj.Width, obj.Height);

        ctx.fillStyle = "white";
        ctx.fillText("Color: " + getColor(), obj.X + obj.Width / 2, 20 + obj.Y + obj.Height / 2);
    }

    //ctx.drawImage(img, 0, 0);

    console.log('Bounding boxes:', objectDesc);
}