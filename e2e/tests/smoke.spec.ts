import { test, expect } from '@playwright/test';

test.describe('Home page', () => {
  test('loads and shows the page title', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveTitle(/Amen/);
  });

  test('displays a verse from the API', async ({ page }) => {
    await page.goto('/');
    // Wait for verse text to appear (a bible reference like "John 3:16")
    const reference = page.locator('text=/\\w+ \\d+:\\d+/');
    await expect(reference.first()).toBeVisible({ timeout: 10_000 });
  });

  test('shows the Amen button', async ({ page }) => {
    await page.goto('/');
    const amenButton = page.getByRole('button', { name: /amen/i });
    await expect(amenButton).toBeVisible({ timeout: 10_000 });
  });
});

test.describe('Admin page', () => {
  test('loads the admin page', async ({ page }) => {
    await page.goto('/admin');
    await expect(page).toHaveTitle(/Amen/);
  });
});

test.describe('API health', () => {
  test('health endpoint returns healthy', async ({ request }) => {
    const response = await request.get('/api/health');
    expect(response.ok()).toBeTruthy();
    const body = await response.json();
    expect(body.status).toBe('healthy');
  });

  test('verse endpoint returns a verse', async ({ request }) => {
    const response = await request.get('/api/verse/random?lang=en');
    expect(response.ok()).toBeTruthy();
    const body = await response.json();
    expect(body.text).toBeTruthy();
    expect(body.reference).toBeTruthy();
    expect(body.index).toBeGreaterThanOrEqual(0);
  });
});
