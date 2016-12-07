﻿IoTApp.createModule('IoTApp.JobProperties', function () {
    "use strict";

    var self = this;

    var init = function (jobId, updateCallback) {
        self.jobId = jobId;
        self.updateCallback = updateCallback;
        getJobPropertiesView();
    }

    var getJobPropertiesView = function () {
        $('#loadingElement').show();

        $.ajaxSetup({ cache: false });
        $.get('/Job/GetJobProperties', { jobId: self.jobId }, function (response) {
            if (!$(".details_grid").is(':visible')) {
                IoTApp.JobIndex.toggleProperties();
            }
            onJobPropertiesDone(response);
        }).fail(function (response) {
            $('#loadingElement').hide();
            IoTApp.Helpers.RenderRetryError(resources.unableToRetrieveRuleFromService, $('#details_grid_container'), function () { getJobPropertiesView(); });
        });
    }

    var onJobPropertiesDone = function (html) {
        $('#loadingElement').hide();
        $('#details_grid_container').empty();
        $('#details_grid_container').html(html);

        $('#showSucceededJobResultsLink').on('click', function () {
            showJobResult($(this), resources.completed);
        });

        $('#showFailedJobResultsLink').on('click', function () {
            showJobResult($(this), resources.failed);
        });

        $('#showPendingJobResultsLink').on('click', function () {
            showJobResult($(this), resources.pending);
        });

        $('#showRunningJobResultsLink').on('click', function () {
            showJobResult($(this), resources.running);
        });

        $('.grid_detail_job_result_hide_link').on('click', function () {
            $(this).parent().children('.grid_detail_job_result_group').toggle();
        });

        $('.toggle_source').on('click', function () {
            $(this).parent().children('.toggle_target').toggle();
            $(this).parent().next().toggle();
        });

        var cancelButton = $('#cancelJobAction');
        if (cancelButton != null) {
            cancelButton.on("click", function () {
                $.ajax({
                    dataType: 'json',
                    type: 'PUT',
                    url: '/api/v1/jobs/' + self.jobId + '/cancel',
                    cache: false,
                    success: self.updateCallback,
                    error: function () {
                        IoTApp.Helpers.Dialog.displayError(resources.failedToCancelJob);
                    }
                });
            });
        }

        setDetailsPaneLoaderHeight();
    }

    var setDetailsPaneLoaderHeight = function () {
        /* Set the height of the Device Details progress animation background to accommodate scrolling */
        var progressAnimationHeight = $("#details_grid_container").height() + $(".details_grid__grid_subhead.button_details_grid").outerHeight();

        $(".loader_container_details").height(progressAnimationHeight);
    };

    var onBegin = function () {
        $('#button_job_status').attr("disabled", "disabled");
    }

    var onSuccess = function (result) {
        $('#button_job_status').removeAttr("disabled");
        if (result.success) {
            self.updateCallback();
        } else if (result.error) {
            IoTApp.Helpers.Dialog.displayError(result.error);
        } else {
            IoTApp.Helpers.Dialog.displayError(resources.jobUpdateError);
        }
    }

    var onFailure = function (result) {
        $('#button_job_status').removeAttr("disabled");
        IoTApp.Helpers.Dialog.displayError(resources.jobUpdateError);
    }

    var onComplete = function () {
        $('#loadingElement').hide();
    }

    var showJobResult = function ($element, status) {
        $element.toggle();
        $element.siblings('.grid_detail_job_result_group').toggle();
        if ($element.siblings('.grid_detail_job_result_list').has('ul').length > 0) {
            return;
        }

        $element.siblings('.job_result_loader_container').show();
        $.ajax({
            url: '/api/v1/jobs/' + self.jobId + '/' + status,
            type: 'GET',
            cache: false,
            success: function (response) {
                $element.siblings('.job_result_loader_container').hide();
                $element.siblings('.grid_detail_job_result_list').empty();
                response.data.forEach(function (item, index) {
                    var methodResponse = item.outcome.deviceMethodResponse;
                    var payload = methodResponse && methodResponse.payload && methodResponse.payload.substring(methodResponse.payload.lastIndexOf('{'));
                    var deviceItem = $('<ul />')
                        .addClass('job_result_section__device_list')
                        .appendTo($element.siblings('.grid_detail_job_result_list'))
                    $('<li />')
                        .attr('title', item.deviceId)
                        .text(IoTApp.Helpers.String.renderLongString(item.deviceId, 20, '..'))
                        .appendTo(deviceItem);
                    $('<li />')
                        .attr('title', payload)
                        .text(IoTApp.Helpers.String.renderLongString(payload, 24, '..'))
                        .appendTo(deviceItem);
                });
            },
            error: function (response) {
                IoTApp.Helpers.RenderRetryError(resources.unableToRetrieveJobFromService, $('#details_grid_container'), function () { getJobPropertiesView(); });
            }
        });
    }

    return {
        init: init,
        onBegin: onBegin,
        onSuccess: onSuccess,
        onFailure: onFailure,
        onComplete: onComplete
    }
}, [jQuery, resources]);