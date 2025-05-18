import { TestBed } from '@angular/core/testing';

import { ProblemDetailsHandlerService } from './problem-details-handler.service';

describe('ProblemDetailsHandlerService', () => {
  let service: ProblemDetailsHandlerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProblemDetailsHandlerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
