export interface Pagination{
    currentPage: number;
    itemsPerPAge: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T>{
    result: T;
    pagination: Pagination
}