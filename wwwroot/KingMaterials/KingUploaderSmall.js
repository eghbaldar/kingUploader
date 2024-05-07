//------------------------------------------------------------- variables Section
const eachCHUNK_smallModule = 1000; // Based on KB // e.g. if you add 1000 in this variable, it is equalavent to (1000*1024)=>Byte // the volume of each chunk for uploading // NOTE: the last part may less than 100kb
var start_smallModule = 0; // first Byte of your file // this varibale will be increase by [chunkSize_smallModule]
var chunkSize_smallModule; // (BYTE) // keep the volume of each chunk in BYTE (eachCHUNK_smallModule * 1024)
var file_smallModule; // Your file
var increased_value_smallModule; // main Progress Global Variable
var current_progress_smallModule = 0;// main Progress Global Variable
var filePartCount_smallModule; // All parts of files after separating
// progressbar section

//-------------------------------------------------------------
const fileInput_smallModule = document.getElementById('fileuploadSmall');
fileInput_smallModule.addEventListener('change', handleFileUpload_smallModule);
//-------------------------------------------------------------
// get the fileupload handler
function handleFileUpload_smallModule(event) {
    ///// check the film extension and size
    var checkSomeErrors = checkStandardVolumeExtentsion_smallModule(fileInput_smallModule);
    if (checkSomeErrors) {
        alert(checkSomeErrors);
        return;
    }
    // end
    file_smallModule = event.target.files[0]; // get first file from the [input file]
    chunkSize_smallModule = 1024 * eachCHUNK_smallModule; // size of each chunk (1MB)
    filePartCount_smallModule = Math.ceil(file_smallModule.size / chunkSize_smallModule);
    CalcIncreaseValue_smallModule(file_smallModule.size); // [increased value] to progress of progressbar
    actionProgressbar_smallModule(true); // the main Progressbar is start_smallModuleed!
    var chunk = file_smallModule.slice(start_smallModule, start_smallModule + chunkSize_smallModule);
    uploadChunk_smallModule(chunk, chunkSize_smallModule, file_smallModule.name, filePartCount_smallModule); // the main function is fired!            
}

//upload
function uploadChunk_smallModule(chunk, chunkSize_smallModule, filename, filePartCount_smallModule) {
    var postData = new FormData();
    postData.append("file", chunk);
    postData.append("chunkSize", chunkSize_smallModule);
    postData.append("Filename", filename);
    postData.append("start", start_smallModule);
    postData.append("filePartCount", filePartCount_smallModule);

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
                start_smallModule += chunkSize_smallModule; // chunkSize_smallModule is not interval, is an index!

                var chunk2 = file_smallModule.slice(start_smallModule, start_smallModule + chunkSize_smallModule);

                if (chunk2.size >= chunkSize_smallModule) {
                    actionProgressbar_smallModule(true);
                }
                else {
                    chunkSize_smallModule = chunk2.size;
                    chunk2 = file_smallModule.slice(start_smallModule, start_smallModule + chunkSize_smallModule);
                    actionProgressbar_smallModule(false); // if the last part is less than [eachCHUNK_smallModule] KB
                }

                if (start_smallModule < file_smallModule.size) {
                    uploadChunk_smallModule(chunk2, chunkSize_smallModule, file_smallModule.name, filePartCount_smallModule);
                }
            }
            if (data.result == 2) {
                actionProgressbar_smallModule(false);
            }
        },
        error: function (request, status, error) {
        }
    });
}

//Progressbar actio
function actionProgressbar_smallModule(lastPartState) {
    if (lastPartState) {
        current_progress_smallModule = current_progress_smallModule + increased_value_smallModule;
        var showedValue = parseFloat(current_progress_smallModule.toFixed(2));
        $("#curprogress").css("width", `${current_progress_smallModule}%`)
        $("#progressNumber").text(`${showedValue}%`);
    }
    else {
        $("#curprogress").css("width", "100%");
        $("#progressNumber").text("100%");
    }
}
// check the file size and extension
function checkStandardVolumeExtentsion_smallModule(fileInput_smallModule) {

    var eVolume = "200000000";
    var eExtension = "jpg";

    var file_extension = /[^.]+$/.exec(fileInput_smallModule.files[0].name); // get only the file extension

    // check the file size at first
    if (fileInput_smallModule.files[0].size < eVolume) {
        // check the file extension
        if (eExtension.toLowerCase() == file_extension.toString().toLowerCase()) {
            return null;// everything is Ok
        }
        else {
            fileInput_smallModule.value = null;
            return "(client side) => Check your file extension!"; // error
        }
    }
    else {
        fileInput_smallModule.value = null;
        return "(client side) => Check your file size!"; // error
    }
}

// CalcIncreaseValue_smallModule
function CalcIncreaseValue_smallModule(fileSize) {
    var kb = fileSize / 1024; // convert to KB
    var eachKb = Math.ceil(kb / eachCHUNK_smallModule); // how many places does it have in 100% progressbar?
    var eachUnit = 100 / eachKb; // how much KB does include in each part (eachKb)?
    increased_value_smallModule = eachUnit;
}

// delete database and files
function DeleteDatabaseAndFiles_smallModule() {
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

// merging_smallModule
function merging_smallModule() {
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