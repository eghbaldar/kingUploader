//------------------------------------------------------------- variables Section
const eachCHUNK_smallModule = 1000; // Based on KB // e.g. if you add 1000 in this variable, it is equalavent to (1000*1024)=>Byte // the volume of each chunk for uploading // NOTE: the last part may less than 100kb
var start_smallModule = 0; // first Byte of your file // this varibale will be increase by [chunkSize_smallModule]
var chunkSize_smallModule; // (BYTE) // keep the volume of each chunk in BYTE (eachCHUNK_smallModule * 1024)
var file_smallModule; // Your file
var increased_value_smallModule; // main Progress Global Variable
var current_progress_smallModule = 0;// main Progress Global Variable
var filePartCount_smallModule; // All parts of files after separating
// progressbar section

//////////////////////////////////////////////////////
var fileStorage = []; // retrive from fileupload array
//////////////////////////////////////////////////////
$('.kingMultiUplaoder').each(function () {
    const fileInput = $(this).find('input[type="file"]');
    fileInput.on('change', function () {
        var fileInputId = $(this).attr("id");
        var file = $(this)[0].files[0];
        var fileEntry = {
            key: fileInputId,
            value: file
        };
        fileStorage.push(fileEntry); // Store the file entry in the fileStorage array
        handleFileUpload_smallModule(fileInputId);
    });

});
//-------------------------------------------------------------
//const fileInput_smallModule = document.getElementById('fileuploadSmall');
//fileInput_smallModule.addEventListener('change', handleFileUpload_smallModule);
//-------------------------------------------------------------
// get the fileupload handler

function handleFileUpload_smallModule(fileInputId) {
    //////////////////////////////////////////////////////// retrive from fileupload array
    var fileEntry = fileStorage.find(function (entry) {
        return entry.key === fileInputId;
    });
    var file = fileEntry.value; // Retrieve the file object from the file entry
    ////////////////////////////////////////////////////

    ///// check the file extension and size
    var checkSomeErrors = checkStandardVolumeExtentsion_smallModule(file);
    if (checkSomeErrors) {
        alert(checkSomeErrors);
        return;
    }
    // end    
    chunkSize_smallModule = 1024 * eachCHUNK_smallModule; // size of each chunk (1MB)
    filePartCount_smallModule = Math.ceil(file.size / chunkSize_smallModule);
    CalcIncreaseValue_smallModule(file.size); // [increased value] to progress of progressbar
    actionProgressbar_smallModule(true, fileInputId); // the main Progressbar is started!
    var chunk = file.slice(start_smallModule, start_smallModule + chunkSize_smallModule);    
    uploadChunk_smallModule(chunk, chunkSize_smallModule, file.name, filePartCount_smallModule, fileInputId); // the main function is fired!
}

//upload
function uploadChunk_smallModule(chunk, chunkSize_smallModule, filename, filePartCount_smallModule, fileInputId) {
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

            /////////////////////
            
            var fileEntry = fileStorage.find(function (entry) {
                return entry.key === fileInputId;
            });
            var file = fileEntry.value; // Retrieve the file object from the file entry
            /////////////////////

            if (data.result == 1) {
                start_smallModule += chunkSize_smallModule; // chunkSize_smallModule is not an interval, but an index!

                var chunk2 = file.slice(start_smallModule, start_smallModule + chunkSize_smallModule);

                if (chunk2.size >= chunkSize_smallModule) {
                    actionProgressbar_smallModule(true, fileInputId);
                }
                else {
                    chunkSize_smallModule = chunk2.size;
                    chunk2 = file.slice(start_smallModule, start_smallModule + chunkSize_smallModule);
                    actionProgressbar_smallModule(false, fileInputId); // if the last part is less than [eachCHUNK_smallModule] KB
                }

                if (start_smallModule < file.size) {
                    uploadChunk_smallModule(chunk2, chunkSize_smallModule, file.name, filePartCount_smallModule, fileInputId);
                }
            }
            if (data.result == 2) {
                actionProgressbar_smallModule(false, fileInputId);
            }
        },
        error: function (request, status, error) {
        }
    });
}

//Progressbar actio
function actionProgressbar_smallModule(lastPartState, fileInputId) {
    if (lastPartState) {

        current_progress_smallModule = current_progress_smallModule + increased_value_smallModule;
        var showedValue = parseFloat(current_progress_smallModule.toFixed(2));

        
        var progressNumber = $('.' + fileInputId).siblings('.progressNumber').attr('id');
        var curprogress = $('.' + fileInputId).siblings('.curprogress').attr('id');


        $("#" + curprogress).css("width", `${current_progress_smallModule}%`);
        $("#" + progressNumber).text(`${showedValue}%`);
    }
    else {
        $("#" + curprogress).css("width", "100%");
        $("#" + progressNumber).text("100%");
    }
}




// check the file size and extension
function checkStandardVolumeExtentsion_smallModule(fileInput) {

    var eVolume = "200000000";
    var eExtension = "jpg";

    var file_extension = /[^.]+$/.exec(fileInput.name); // get only the file extension

    // check the file size at first
    if (fileInput.size < eVolume) {
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