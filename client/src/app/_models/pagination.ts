export interface Pagination {
    CurrentPage :number;
    ItemsPerPage :number;
    TotalItems :number;
    TotalPage :number;
}

export class PaginatedResult<T> {
    result: T | null;
    pagination : Pagination;
}