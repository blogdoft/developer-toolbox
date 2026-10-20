import { parseProblemDetails } from './problem-details-handler.service';

describe('parseProblemDetails', () => {
    it('should list title and detail', () => {
        expect(parseProblemDetails({ title: 'Not found', detail: 'The queue does not exist.' })).toBe('Not found\nThe queue does not exist.');
    });

    it('should list every validation error', () => {
        const text = parseProblemDetails({ title: 'Invalid', errors: { name: ['is required'] } });

        expect(text).toContain('Invalid');
        expect(text).toContain(' • name: is required');
    });

    it('should fall back to the instance when there is nothing to show', () => {
        expect(parseProblemDetails({ instance: '/api/x' })).toBe('Unknown error happened.\nInstance: /api/x');
    });
});
