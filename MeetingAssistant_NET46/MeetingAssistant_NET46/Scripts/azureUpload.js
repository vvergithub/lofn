var reader;
var filename;
var sContainer;
var maxSliceSize = 256 * 32;
var selectedFile = null;
var sliceIds = new Array();
var upFile;

function handleFileUpload(cnt, sType) {
    var files = cnt.files; // FileList object
    selectedFile = files[0];

    //----------------------------CHECKS---------------------------
    //Check whether there is a file to upload
    if (files.length === 0) { return; }

    // Check for the various File API support.
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        // Great success! All the File APIs are supported.
    } else {
        alert('The File APIs are not fully supported in this browser.');
        return;
    }

    //test whether this is an image file
    rFilter = /^(?:image\/bmp|image\/cis\-cod|image\/gif|image\/ief|image\/jpeg|image\/jpeg|image\/jpeg|image\/pipeg|image\/png|image\/svg\+xml|image\/tiff|image\/x\-cmu\-raster|image\/x\-cmx|image\/x\-icon|image\/x\-portable\-anymap|image\/x\-portable\-bitmap|image\/x\-portable\-graymap|image\/x\-portable\-pixmap|image\/x\-rgb|image\/x\-xbitmap|image\/x\-xpixmap|image\/x\-xwindowdump)$/i;
    if (!rFilter.test(selectedFile.type)) { alert("You must select a valid image file!"); return; }

    //----------------------------UPLOAD---------------------------
    //Create a name for the blob
    filename = selectedFile.name.toLowerCase();
    sContainer = "images";

    //Upload the file
    reader = new FileReader();

    reader.onloadend = function (evt) {
        if (evt.target.readyState == FileReader.DONE) { // DONE == 2
            //Initialise variables
            maxSliceSize = 256 * 32;
            upFile = evt.target.result;
            sliceIds = new Array();

            uploadFileInSlices();
        }
    }

    reader.readAsDataURL(selectedFile);
}

function pad(number, length) {
    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }
    return str;
}

function uploadFileInSlices() {
    if (upFile != "") {
        var sliceId = pad(sliceIds.length, 6);
        console.log("slice id = " + sliceId);
        sliceIds.push(sliceId);
        //Send the first slice off to the server and remove it from the file string
        var upSlice = upFile.substring(0, maxSliceSize);
        upFile = upFile.substring(maxSliceSize);

        var params = {
            filename: filename,
            sliceID: sliceId,
            upSlice: upSlice
        };
        proxy.invoke("UploadImageSlice", params, uploadFileInSlices, onProxyFailure, true);
    } else {
        commitSliceList();
    }
}

function commitSliceList() {
    var jsonData = []; //declare object
    for (var i = 0; i < sliceIds.length; i++) {
        jsonData.push({ SliceName: sliceIds[i] });
    }
    console.log(jsonData);
    var params = {
        filename: filename,
        sliceList: jsonData,
        upFileType: selectedFile.type,
        sContainer: sContainer
    };
    proxy.invoke("UploadImage", params, onSuccess, onProxyFailure, true);
}