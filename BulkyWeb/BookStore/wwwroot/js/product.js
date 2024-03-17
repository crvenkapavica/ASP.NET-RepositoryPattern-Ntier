$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    let dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall'},
        "columns": [
            { data: 'title', "width": "15%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "15%", "text-align": "center" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "15%" }
        ]
    });
}   

