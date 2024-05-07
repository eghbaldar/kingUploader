//------------------------------------------------------------- variables Section
const eachCHUNK = 1000; // Based on KB // e.g. if you add 1000 in this variable, it is equalavent to (1000*1024)=>Byte // the volume of each chunk for uploading // NOTE: the last part may less than 100kb
var start = 0; // first Byte of your file // this varibale will be increase by [chunkSize]
var chunkSize; // (BYTE) // keep the volume of each chunk in BYTE (eachCHUNK * 1024)
var file; // Your file
var increased_value; // main Progress Global Variable
var current_progress = 0;// main Progress Global Variable
var filePartCount; // All parts of files after separating
// progressbar section

//-------------------------------------------------------------
const fileInput = document.getElementById('fileuploadSmall');
fileInput.addEventListener('change', handleFileUpload);
//-------------------------------------------------------------
// get the fileupload handler
function handleFileUpload(event) {
    ///// check the film extension and size
    var checkSomeErrors = checkStandardVolumeExtentsion(fileInput);
    if (checkSomeErrors) {
        alert(checkSomeErrors);
        return;
    }
    // end
    file = event.target.files[0]; // get first file from the [input file]
    chunkSize = 1024 * eachCHUNK; // size of each chunk (1MB)
    filePartCount = Math.ceil(file.size / chunkSize);
    CalcIncreaseValue(file.size); // [increased value] to progress of progressbar
    actionProgressbar(true); // the main Progressbar is started!
    var chunk = file.slice(start, start + chunkSize);
    uploadChunk(chunk, chunkSize, file.name, filePartCount); // the main function is fired!            
}

//upload
function uploadChunk(chunk, chunkSize, filename, filePartCount) {
    var postData = new FormData();
    postData.append("file", chunk);
    postData.append("ChunkSize", chunkSize);
    postData.append("Filename", filename);
    postData.append("Start", start);
    postData.append("FilePartCount", filePartCount);

    $.ajax({
        contentType: false,
        processData: false,
        type: 'POST',
        data: postData,
        url: '/Home/Upload',
        success: function (data) {
            if (data.result == 0) {
                alert(data.message);
                location.reload();
            }
            if (data.result == 1) {
                start += chunkSize; // chunkSize is not interval, is an index!

                var chunk2 = file.slice(start, start + chunkSize);

                if (chunk2.size >= chunkSize) {
                    actionProgressbar(true);
                }
                else {
                    chunkSize = chunk2.size;
                    chunk2 = file.slice(start, start + chunkSize);
                    actionProgressbar(false); // if the last part is less than [eachCHUNK] KB
                }

                if (start < file.size) {
                    uploadChunk(chunk2, chunkSize, file.name, filePartCount);
                }
            }
            if (data.result == 2) {
                actionProgressbar(false);
            }
        },
        error: function (request, status, error) {
        }
    });
}

//Progressbar actio
function actionProgressbar(lastPartState) {
    if (lastPartState) {
        current_progress = current_progress + increased_value;
        var showedValue = parseFloat(current_progress.toFixed(2));
        $("#curprogress").css("width", `${current_progress}%`)
        $("#progressNumber").text(`${showedValue}%`);
    }
    else {
        $("#curprogress").css("width", "100%");
        $("#progressNumber").text("100%");
    }
}
// check the file size and extension
function checkStandardVolumeExtentsion(fileInput) {

    var eVolume = "200000000";
    var eExtension = "jpg";

    var file_extension = /[^.]+$/.exec(fileInput.files[0].name); // get only the file extension

    // check the file size at first
    if (fileInput.files[0].size < eVolume) {
        // check the file extension
        if (eExtension.toLowerCase() == file_extension.toString().toLowerCase()) {
            return null;// everything is Ok
        }
        else {
            fileInput.value = null;
            return "(client side) => Check your file extension!"; // error
        }
    }
    else {
        fileInput.value = null;
        return "(client side) => Check your file size!"; // error
    }
}

// CalcIncreaseValue
function CalcIncreaseValue(fileSize) {
    var kb = fileSize / 1024; // convert to KB
    var eachKb = Math.ceil(kb / eachCHUNK); // how many places does it have in 100% progressbar?
    var eachUnit = 100 / eachKb; // how much KB does include in each part (eachKb)?
    increased_value = eachUnit;
}

// delete database and files
function DeleteDatabaseAndFiles() {
    $.ajax({
        content: 'application/x-www-form-urlencoded',
        contentType: 'json',
        type: 'POST',
        url: '/Home/Delete',
        success: function (data) {
        },
        error: function (request, status, error) {
            //alert('request:' + request.responseText + ';err:' + error);
        }
    });
}

// Merging
function merging() {
    $.ajax({
        contentType: false,
        dataType: false,
        type: 'POST',
        url: '/Home/Merge',
        success: function () {
            //alert('1');
        },
        error: function (request, status, error) {
            //alert('request:' + request.responseText + ';err:' + error);
        }
    });
}